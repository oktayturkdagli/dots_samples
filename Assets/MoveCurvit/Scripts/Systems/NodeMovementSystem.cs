using System.Runtime.InteropServices;
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
    [StructLayout(LayoutKind.Auto)]
    public partial struct NodeMovementSystem : ISystem
    {
        private EntityQuery nodeEntitiesQuery;
            
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
            state.RequireForUpdate<SelectedTag>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            
            state.RequireForUpdate<NodeComponent>();
            state.RequireForUpdate<LocalTransform>();
            
            nodeEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<NodeComponent, LocalTransform>()
                .Build(ref state);
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
            var movementDirection = new float3(inputComponent.AxisX * deltaTime, 0, inputComponent.AxisY * deltaTime) * 10f;
            
            ScheduleJobs(ref state, movementDirection);
        }

        [BurstCompile]
        private void ScheduleJobs(ref SystemState state, float3 movementDirection)
        {
            var nodeMovementParallelJobHandle = ScheduleNodeMoveJob(ref state, movementDirection);
            state.Dependency = nodeMovementParallelJobHandle;
        }
        
        [BurstCompile]
        private JobHandle ScheduleNodeMoveJob(ref SystemState state, float3 movementDirection)
        {
            var nodeComponents = nodeEntitiesQuery.ToComponentDataArray<NodeComponent>(Allocator.TempJob);
            var nodeLocalTransforms = nodeEntitiesQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
         
            var nodeMoveParallelJobHandle = new NodeMovementParallelJob
            {
                NodeComponentsNativeArray = nodeComponents,
                LocalTransformsNativeArray = nodeLocalTransforms,
                MovementDirection = movementDirection,
            }.Schedule(nodeComponents.Length, nodeComponents.Length / 4, state.Dependency);
           
            nodeEntitiesQuery.CopyFromComponentDataArray(nodeComponents);
            nodeEntitiesQuery.CopyFromComponentDataArray(nodeLocalTransforms);

            nodeComponents.Dispose();
            nodeLocalTransforms.Dispose();
           
            return nodeMoveParallelJobHandle;
        }
    }
}