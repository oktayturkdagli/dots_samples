using MoveElements.Scripts.Components;
using MoveElements.Scripts.Data;
using MoveElements.Scripts.Jobs;
using MoveElements.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace MoveElements.Scripts.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct ElementMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
            state.RequireForUpdate<SelectedTag>();
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var inputComponent = SystemAPI.GetSingleton<InputComponent>();
            ScheduleJobs(ref state, deltaTime, inputComponent);
        }

        [BurstCompile]
        private void ScheduleJobs(ref SystemState state, float deltaTime, InputComponent inputComponent, float speed = 10f)
        {
            var elementMovementParallelJobHandle = ScheduleElementMoveJob(ref state, deltaTime, inputComponent, speed);
            state.Dependency = elementMovementParallelJobHandle;
            elementMovementParallelJobHandle.Complete();
        }
        
        [BurstCompile]
        private JobHandle ScheduleElementMoveJob(ref SystemState state, float deltaTime, InputComponent inputComponent, float speed = 10f)
        {
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<ElementComponent, LocalTransform>()
                .Build(state.EntityManager);
            
            var movementDirection = new float3(inputComponent.axisX * deltaTime, 0, inputComponent.axisY * deltaTime) * speed;
            var elementMoveParallelJobHandle = new ElementMovementParallelJob
            {
                MovementDirection = movementDirection
            }.ScheduleParallel(query, state.Dependency);
            
            return elementMoveParallelJobHandle;
        }
    }
}