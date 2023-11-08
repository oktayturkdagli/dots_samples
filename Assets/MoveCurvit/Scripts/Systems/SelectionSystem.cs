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
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                SelectNodes(ref state, ref ecb);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                UnselectNodes(ref state, ref ecb);
            }
        }

        [BurstCompile]
        private void SelectNodes(ref SystemState state, ref EntityCommandBuffer ecb)
        {
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NodeComponent>()
                .WithNone<BuildVisualTag, SelectedTag>()
                .Build(state.EntityManager);
            
            var entityArray = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entityArray)
            {
                ecb.AddComponent<SelectedTag>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
        
        [BurstCompile]
        private void UnselectNodes(ref SystemState state, ref EntityCommandBuffer ecb)
        {
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<NodeComponent, SelectedTag>()
                .Build(state.EntityManager);
            
            var entityArray = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entityArray)
            {
                ecb.RemoveComponent<SelectedTag>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}