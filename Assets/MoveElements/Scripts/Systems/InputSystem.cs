using MoveElements.Scripts.Components;
using MoveElements.Scripts.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MoveElements.Scripts.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginInitializationEntityCommandBufferSystem))]
    [UpdateBefore(typeof(ElementCreateSystem))]
    public partial struct InputSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
            var inputEntity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(inputEntity, new InputComponent());
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                var query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<ElementComponent>()
                    .WithNone<SelectedTag>()
                    .Build(state.EntityManager);
            
                var entityArray = query.ToEntityArray(Allocator.Temp);
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                foreach (var entity in entityArray)
                {
                    ecb.AddComponent<SelectedTag>(entity);
                }
                ecb.Playback(state.EntityManager);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                var query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<ElementComponent, SelectedTag>()
                    .Build(state.EntityManager);
            
                var entityArray = query.ToEntityArray(Allocator.Temp);
                var ecb = new EntityCommandBuffer(Allocator.Temp);
                foreach (var entity in entityArray)
                {
                    ecb.RemoveComponent<SelectedTag>(entity);
                }
                ecb.Playback(state.EntityManager);
            }
            else
            {
                var horizontalInput = Input.GetAxis("Horizontal");
                var verticalInput = Input.GetAxis("Vertical");
            
                state.EntityManager.SetComponentData(
                    SystemAPI.GetSingletonEntity<InputComponent>(),
                    new InputComponent 
                    {
                        axisX = horizontalInput, 
                        axisY = verticalInput
                    }
                );
            }
        }
    }
}