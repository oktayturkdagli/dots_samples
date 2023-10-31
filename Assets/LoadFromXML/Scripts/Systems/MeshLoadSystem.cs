using LoadFromXML.Scripts.Extensions;
using LoadFromXML.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace LoadFromXML.Scripts.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct MeshLoadSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LoadComponent>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    
        public void OnUpdate(ref SystemState state)
        {
            var laneletMaterial = Resources.Load<Material>($"Materials/MAT_Lanelet");
            var desc = new RenderMeshDescription(ShadowCastingMode.Off, false, MotionVectorGenerationMode.ForceNoMotion);
            var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
            var query = SystemAPI.QueryBuilder().WithAll<LaneletComponent>().Build();
            var entities = query.ToEntityArray(Allocator.Temp);
            var loadComponent = SystemAPI.GetSingleton<LoadComponent>();
        
            var firstLaneletID = loadComponent.LaneletDataNativeArray[0].ID;
            var firstWayID = loadComponent.WayDataNativeArray[0].ID;
            var leftVerticesNativeList = new NativeList<float3>(64,Allocator.TempJob);
            var rightVerticesNativeList = new NativeList<float3>(64,Allocator.TempJob);

            foreach (var entity in entities)
            {
                leftVerticesNativeList.Clear();
                rightVerticesNativeList.Clear();
            
                var laneletComponent = state.EntityManager.GetComponentData<LaneletComponent>(entity);
                var leftWay = loadComponent.WayDataNativeArray[loadComponent.LaneletDataNativeArray[laneletComponent.ID - firstLaneletID].LeftWayId - firstWayID];
                var rightWay = loadComponent.WayDataNativeArray[loadComponent.LaneletDataNativeArray[laneletComponent.ID - firstLaneletID].RightWayId - firstWayID];
            
                for (var i = 0; i < leftWay.NodeCount; i++)
                    leftVerticesNativeList.Add(
                        loadComponent.NodeDataNativeArray[loadComponent.NodeListForWayNativeList[leftWay.SliceStartId + i] - 1].Position
                    );
            
                for (var i = 0; i < rightWay.NodeCount; i++)
                    rightVerticesNativeList.Add(
                        loadComponent.NodeDataNativeArray[loadComponent.NodeListForWayNativeList[rightWay.SliceStartId + i] - 1].Position
                    );
            
                var mesh = MeshExtension.BuildMeshForLanelet(leftVerticesNativeList, rightVerticesNativeList);
                if (laneletMaterial == null || mesh == null)
                    return;
                var meshArray = new RenderMeshArray(new[] { laneletMaterial }, new[] { mesh });
                RenderMeshUtility.AddComponents(entity, state.EntityManager, desc, meshArray, materialMeshInfo);
            }
        }
    }
}