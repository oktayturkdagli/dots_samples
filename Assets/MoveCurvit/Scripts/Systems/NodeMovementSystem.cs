using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Jobs;
using MoveCurvit.Scripts.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace MoveCurvit.Scripts.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial struct NodeMovementSystem : ISystem
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
            var nodeMovementParallelJobHandle = ScheduleNodeMoveJob(ref state, deltaTime, inputComponent, speed);
            state.Dependency = nodeMovementParallelJobHandle;
            nodeMovementParallelJobHandle.Complete();
        }
        
        [BurstCompile]
        private JobHandle ScheduleNodeMoveJob(ref SystemState state, float deltaTime, InputComponent inputComponent, float speed = 10f)
        {
            var query = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<NodeComponent, LocalTransform>()
                .Build(state.EntityManager);
            
            var movementDirection = new float3(inputComponent.AxisX * deltaTime, 0, inputComponent.AxisY * deltaTime) * speed;
            var nodeMoveParallelJobHandle = new NodeMovementParallelJob
            {
                MovementDirection = movementDirection
            }.ScheduleParallel(query, state.Dependency);
            
            return nodeMoveParallelJobHandle;
        }
    }
}