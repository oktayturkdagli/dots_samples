using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Components.Buffers;
using MoveCurvit.Scripts.Data;
using MoveCurvit.Scripts.Tags;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MoveCurvit.Scripts.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
    [StructLayout(LayoutKind.Auto)]
    public partial struct CurvitLoadingSystem : ISystem
    {
        private NativeHashMap<uint, Entity> nodeHashMap;
        private NativeHashMap<uint, Entity> wayHashMap;
    
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CurvitComponent>();
            state.RequireForUpdate<CurvitPathComponent>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var curvitPathEntity = SystemAPI.GetSingletonEntity<CurvitPathComponent>();
            var curvitPathComponentSingleton = state.EntityManager.GetComponentData<CurvitPathComponent>(curvitPathEntity);
            var curvitComponentSingleton = SystemAPI.GetSingleton<CurvitComponent>();
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(curvitPathComponentSingleton.CurvitLoadPath.ToString());
            
            var curvitNode = xmlDocument.SelectSingleNode("osm");
            var nodeList = curvitNode!.SelectNodes("node");
            var wayList = curvitNode.SelectNodes("way");
            var laneletList = curvitNode.SelectNodes("relation");
            
            CreateNodeEntities(ecb, nodeList, curvitComponentSingleton.NodeEntity, curvitComponentSingleton.NodeEntityScale);
            CreateWayEntities(ecb, wayList, curvitComponentSingleton.WayEntity);
            CreateLaneletEntities(ecb, laneletList, curvitComponentSingleton.LaneletEntity);
            
            ecb.DestroyEntity(curvitPathEntity);
        }

        private void CreateNodeEntities(EntityCommandBuffer ecb, XmlNodeList nodeList, Entity nodeEntityPrefab, float nodeScale)
        {
            nodeHashMap = new NativeHashMap<uint, Entity>(nodeList.Count, Allocator.Persistent);
            
            for (var i = 0; i < nodeList.Count; i++)
            {
                var xmlNode = nodeList[i];
                var id = uint.Parse(xmlNode.Attributes!["id"].Value);
                
                var position = new float3();
                foreach (XmlNode tag in xmlNode.SelectNodes("tag")!)
                {
                    switch (tag.Attributes!["k"].Value)
                    {
                        case "ele":
                            position.y = float.Parse(tag.Attributes["v"].Value);
                            break;
                        case "local_x":
                            position.x = float.Parse(tag.Attributes["v"].Value);
                            break;
                        case "local_y":
                            position.z = float.Parse(tag.Attributes["v"].Value);
                            break;
                    }
                }

                var nodeEntity = ecb.Instantiate(nodeEntityPrefab);
                ecb.AddBuffer<WayBuffer>(nodeEntity);
                ecb.AddComponent(nodeEntity, new NodeComponent
                {
                    ID = id,
                    Position = position
                });
            
                ecb.SetComponent(nodeEntity, new LocalTransform
                {
                    Position = position,
                    Rotation = quaternion.identity,
                    Scale = nodeScale
                });
            
                nodeHashMap.Add(id, nodeEntity);
            }
        }

        private void CreateWayEntities(EntityCommandBuffer ecb, XmlNodeList wayList, Entity wayEntityPrefab)
        {
            wayHashMap = new NativeHashMap<uint, Entity>(wayList.Count, Allocator.Persistent);
            DataHolder.WayToLineRendererDictionary = new Dictionary<uint, LineRendererDataHolder>(wayList.Count);
            
            for (var i = 0; i < wayList.Count; i++)
            {
                XmlNode xmlWay = wayList[i];
                uint id = uint.Parse(xmlWay.Attributes!["id"].Value);
                var xmlWayNodesList = xmlWay.SelectNodes("nd");
                
                var wayEntity = ecb.Instantiate(wayEntityPrefab);
                ecb.AddComponent(wayEntity, new WayComponent
                {
                    ID = id
                });
                
                ecb.AddBuffer<LaneletBuffer>(wayEntity);
                var nodeBuffer = ecb.AddBuffer<NodeBuffer>(wayEntity);
                nodeBuffer.Length = xmlWayNodesList!.Count;

                for (var j = 0; j < xmlWayNodesList.Count; j++)
                {
                    var nodeID = uint.Parse(xmlWayNodesList[j].Attributes!["ref"].Value);
                    nodeHashMap.TryGetValue(nodeID, out var nodeEntity);
                    
                    nodeBuffer[j] = new NodeBuffer { NodeEntity = nodeEntity };
                    ecb.AppendToBuffer(nodeEntity, new WayBuffer{ WayEntity = wayEntity });
                }
                
                ecb.AddComponent<BuildVisualTag>(wayEntity);
                wayHashMap.Add(id, wayEntity);
            }
        }

        private void CreateLaneletEntities(EntityCommandBuffer ecb, XmlNodeList laneletList, Entity laneletEntityPrefab)
        {
            DataHolder.LaneletToMeshDictionary = new Dictionary<uint, LaneletDataHolder>(laneletList.Count);
        
            for (var i = 0; i < laneletList.Count; i++)
            {
                var xmlLanelet = laneletList[i];
                var id = uint.Parse(xmlLanelet.Attributes!["id"].Value);
                
                var laneletEntity = ecb.Instantiate(laneletEntityPrefab);
                ecb.AddBuffer<LeftWayNodePositionBuffer>(laneletEntity);
                ecb.AddBuffer<RightWayNodePositionBuffer>(laneletEntity);
                
                var laneletComponent = new LaneletComponent
                {
                    ID = id
                };
                
                foreach (XmlNode member in xmlLanelet.SelectNodes("member")!)
                {
                    if (member.Attributes!["type"].Value == "way")
                    {
                        var wayID = uint.Parse(member.Attributes["ref"].Value);
                        
                        switch (member.Attributes["role"].Value)
                        {
                            case "left":
                                wayHashMap.TryGetValue(wayID, out var leftWay);
                                laneletComponent.LeftWay = leftWay;
                                ecb.AppendToBuffer(leftWay, new LaneletBuffer
                                {
                                    LaneletEntity = laneletEntity
                                });
                                break;
                            case "right":
                                wayHashMap.TryGetValue(wayID, out var rightWay);
                                laneletComponent.RightWay = rightWay;
                                ecb.AppendToBuffer(rightWay, new LaneletBuffer
                                {
                                    LaneletEntity = laneletEntity
                                });
                                break;
                        }
                    }
                }
                
                ecb.AddComponent(laneletEntity, laneletComponent);
                ecb.AddComponent<BuildVisualTag>(laneletEntity);
            }
        }
    }
}