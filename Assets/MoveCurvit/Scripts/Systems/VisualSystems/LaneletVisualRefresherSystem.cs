using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Components.Buffers;
using MoveCurvit.Scripts.Data;
using MoveCurvit.Scripts.Extensions;
using MoveCurvit.Scripts.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MoveCurvit.Scripts.Systems.VisualSystems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(WayVisualRefresherSystem))]
    public partial struct LaneletVisualRefresherSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectedTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var laneletData in
                     SystemAPI.Query<RefRO<LaneletComponent>>()
                         .WithNone<BuildVisualTag>())
            {
                var leftWayNodeDataBuffer = SystemAPI.GetBuffer<NodeBuffer>(laneletData.ValueRO.LeftWay);
                var rightWayNodeDataBuffer = SystemAPI.GetBuffer<NodeBuffer>(laneletData.ValueRO.RightWay);
                
                NativeList<float3> leftWayPositions = new NativeList<float3>(leftWayNodeDataBuffer.Length, Allocator.Temp);
                NativeList<float3> rightWayPositions = new NativeList<float3>(rightWayNodeDataBuffer.Length, Allocator.Temp);
                
                for (var i = 0; i < leftWayNodeDataBuffer.Length; i++)
                    leftWayPositions.Add(SystemAPI.GetComponentRO<NodeComponent>(leftWayNodeDataBuffer[i].NodeEntity).ValueRO.Position);
                
                for (var i = 0; i < rightWayNodeDataBuffer.Length; i++)
                    rightWayPositions.Add(SystemAPI.GetComponentRO<NodeComponent>(rightWayNodeDataBuffer[i].NodeEntity).ValueRO.Position);
                
                DataHolder.LaneletToMeshDictionary.TryGetValue(laneletData.ValueRO.ID, out var laneletMeshReference);
                MeshExtensions.SetMeshVertices(leftWayPositions, rightWayPositions, laneletMeshReference.Mesh);
            }
        }
    }
}