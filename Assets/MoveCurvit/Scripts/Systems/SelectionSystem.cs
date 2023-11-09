using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MoveCurvit.Scripts.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
    [UpdateAfter(typeof(InputSystem))]
    public partial struct SelectionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionComponent>();
            var inputEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(inputEntity, new SelectionComponent());
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                Debug.Log("N");
                SelectUnselect<NodeComponent, SelectedNodeTag>(ref state, true);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("W");
                SelectUnselect<WayComponent, SelectedWayTag>(ref state, true);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("L");
                SelectUnselect<LaneletComponent, SelectedLaneletTag>(ref state, true);
            }
        }
        
        [BurstCompile]
        private void SelectUnselect<TComponent, TSelectedTag>(ref SystemState state, bool select)
            where TComponent : struct, IComponentData
            where TSelectedTag : unmanaged, IComponentData
        {
            var query = new EntityQueryBuilder(Allocator.TempJob)
                .WithAll<TComponent>()
                .WithNone<BuildVisualTag, TSelectedTag>()
                .Build(state.EntityManager);
            var entities = query.ToEntityArray(Allocator.TempJob);
            
            
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            if (select)
            {
                SelectEntities<TSelectedTag>(ref ecb, ref entities, 16);
            }
            else
            {
                UnselectEntities<TSelectedTag>(ref ecb, ref entities);
            }
            ecb.Playback(state.EntityManager);
            entities.Dispose();
            ecb.Dispose();
        }
        
        [BurstCompile]
        private void SelectEntities<TSelectedTag>(ref EntityCommandBuffer ecb, ref NativeArray<Entity> entities, int count)
            where TSelectedTag : unmanaged, IComponentData
        {
            for (var i = 0; i < Mathf.Min(count, entities.Length); i++)
            {
                ecb.AddComponent<TSelectedTag>(entities[i]);
            }
        }
        
        [BurstCompile]
        private void UnselectEntities<TSelectedTag>(ref EntityCommandBuffer ecb, ref NativeArray<Entity> entities)
            where TSelectedTag : struct, IComponentData
        {
            foreach (var entity in entities)
            {
                ecb.RemoveComponent<TSelectedTag>(entity);
            }
        }
    }
}