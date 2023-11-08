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
    [UpdateBefore(typeof(CurvitLoadingSystem))]
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
            var horizontalInput = Input.GetAxis("Horizontal");
            var verticalInput = Input.GetAxis("Vertical");
            
            state.EntityManager.SetComponentData(SystemAPI.GetSingletonEntity<InputComponent>(), new InputComponent 
                {
                    AxisX = horizontalInput, 
                    AxisY = verticalInput
                }
            );
        }
    }
}