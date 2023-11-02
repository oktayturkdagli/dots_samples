using MoveElements.Scripts.Components;
using MoveElements.Scripts.Data;
using MoveElements.Scripts.Jobs;
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
            state.RequireForUpdate<ElementComponent>();
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            ScheduleJobs(ref state, deltaTime);
        }

        [BurstCompile]
        private void ScheduleJobs(ref SystemState state, float deltaTime)
        {
            var elementMoveParallelJobHandle = ScheduleElementMoveJob(ref state, deltaTime);
            state.Dependency = elementMoveParallelJobHandle;
            elementMoveParallelJobHandle.Complete();
        }
        
        [BurstCompile]
        private JobHandle ScheduleElementMoveJob(ref SystemState state, float deltaTime)
        {
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<ElementComponent, LocalTransform>()
                .Build(state.EntityManager);
            
            var movementDirection = new float3(1 * deltaTime, 0, 0);
            var elementMoveParallelJobHandle = new ElementMoveParallelJob
            {
                MovementDirection = movementDirection
            }.ScheduleParallel(query, state.Dependency);
            
            return elementMoveParallelJobHandle;
        }
    }
}