using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Components.Buffers;
using MoveCurvit.Scripts.Data;
using MoveCurvit.Scripts.Tags;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace MoveCurvit.Scripts.Systems.VisualSystems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial struct WayVisualBuilderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BuildVisualTag>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var lineRendererForWayPrefab = Resources.Load<LineRendererDataHolder>("Prefabs/WayLineRenderer");
            
            foreach (var (wayData, nodeReferenceBuffer, wayEntity) in SystemAPI
                         .Query<RefRO<WayComponent>, DynamicBuffer<NodeBuffer>>()
                         .WithAll<BuildVisualTag>().WithEntityAccess())
            {
                var lineRenderer = Object.Instantiate(lineRendererForWayPrefab);
                lineRenderer.SetLineRendererPositionCount(nodeReferenceBuffer.Length);
                for (var i = 0; i < nodeReferenceBuffer.Length; i++)
                    lineRenderer.SetLineRendererPosition(i, SystemAPI.GetComponentRO<NodeComponent>(nodeReferenceBuffer[i].NodeEntity).ValueRO.Position);
                
                DataHolder.WayToLineRendererDictionary.Add(wayData.ValueRO.ID, lineRenderer);
                ecb.RemoveComponent<BuildVisualTag>(wayEntity);
            }
        }
    }
}