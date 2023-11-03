using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Components.Buffers;
using MoveCurvit.Scripts.Data;
using MoveCurvit.Scripts.Extensions;
using MoveCurvit.Scripts.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace MoveCurvit.Scripts.Systems.VisualSystems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(WayVisualBuilderSystem))]
    public partial struct LaneletVisualBuilderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildVisualTag>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            var hybridRenderer = state.World.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();

            foreach (var (laneletData, materialMeshInfo, laneletEntity) in
                     SystemAPI
                         .Query<RefRO<LaneletComponent>, RefRW<MaterialMeshInfo>>()
                         .WithAll<BuildVisualTag>().WithEntityAccess()) 
            {
                var leftWayNodeDataBuffer = SystemAPI.GetBuffer<NodeBuffer>(laneletData.ValueRO.LeftWay);
                var rightWayNodeDataBuffer = SystemAPI.GetBuffer<NodeBuffer>(laneletData.ValueRO.RightWay);
            
                NativeList<float3> leftWayPositions = new NativeList<float3>(leftWayNodeDataBuffer.Length, Allocator.Temp);
                NativeList<float3> rightWayPositions = new NativeList<float3>(rightWayNodeDataBuffer.Length, Allocator.Temp);

                for (var i = 0; i < leftWayNodeDataBuffer.Length; i++)
                    leftWayPositions.Add(SystemAPI.GetComponentRO<NodeComponent>(leftWayNodeDataBuffer[i].NodeEntity).ValueRO
                        .Position);
            
                for (var i = 0; i < rightWayNodeDataBuffer.Length; i++)
                    rightWayPositions.Add(SystemAPI.GetComponentRO<NodeComponent>(rightWayNodeDataBuffer[i].NodeEntity).ValueRO
                        .Position);

                var mesh = MeshExtensions.BuildMeshForLanelet(leftWayPositions, rightWayPositions);
                var meshBatchID = hybridRenderer.RegisterMesh(mesh);
                var meshReference = new LaneletDataHolder(mesh, meshBatchID);

                materialMeshInfo.ValueRW.MeshID = meshBatchID;
            
                DataHolder.LaneletToMeshDictionary.Add(laneletData.ValueRO.ID, meshReference);
                
                ecb.RemoveComponent<BuildVisualTag>(laneletEntity);
            }
        }
    }
}