using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Components.Buffers;
using MoveCurvit.Scripts.Data;
using MoveCurvit.Scripts.Tags;
using Unity.Burst;
using Unity.Entities;

namespace MoveCurvit.Scripts.Systems.VisualSystems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(NodeMovementSystem))]
    public partial struct WayVisualRefresherSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectedNodeTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (wayData, nodeReferenceBuffer) in SystemAPI
                         .Query<RefRO<WayComponent>, DynamicBuffer<NodeBuffer>>()
                         .WithNone<BuildVisualTag>())
            {
                DataHolder.WayToLineRendererDictionary.TryGetValue(wayData.ValueRO.ID, out var lineRenderer);
                if (lineRenderer == null)
                    return;
            
                for (var i = 0; i < nodeReferenceBuffer.Length; i++)
                    lineRenderer.SetLineRendererPosition(i, SystemAPI.GetComponentRO<NodeComponent>(nodeReferenceBuffer[i].NodeEntity).ValueRO.Position);
            }
        }
    }
}