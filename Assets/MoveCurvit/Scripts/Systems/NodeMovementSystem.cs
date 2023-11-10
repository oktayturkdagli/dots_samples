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
        private EntityQuery wayEntitiesQuery;
        private EntityQuery laneletEntitiesQuery;
            
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InputComponent>();
            state.RequireForUpdate<SelectedNodeTag>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
            
            state.RequireForUpdate<NodeComponent>();
            state.RequireForUpdate<LocalTransform>();
            
            nodeEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<NodeComponent, LocalTransform>()
                .WithAll<SelectedNodeTag>()
                .Build(ref state);
            
            wayEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<WayComponent, LocalTransform>()
                .WithAll<SelectedWayTag>()
                .Build(ref state);
            
            laneletEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAllRW<LaneletComponent, LocalTransform>()
                .WithAll<SelectedLaneletTag>()
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
            var wayMovementParallelJobHandle = ScheduleWayMoveJob(ref state, movementDirection);
            var laneletMovementParallelJobHandle = ScheduleLaneletMoveJob(ref state, movementDirection);
            state.Dependency = JobHandle.CombineDependencies(
                nodeMovementParallelJobHandle, 
                wayMovementParallelJobHandle, 
                laneletMovementParallelJobHandle);
        }
        
        [BurstCompile]
        private JobHandle ScheduleNodeMoveJob(ref SystemState state, float3 movementDirection)
        {
            var nodeComponents = nodeEntitiesQuery.ToComponentDataArray<NodeComponent>(Allocator.TempJob);
            var nodeLocalTransforms = nodeEntitiesQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
         
            var nodeMovementParallelJobHandle = new NodeMovementParallelJob
            {
                NodeComponentsNativeArray = nodeComponents,
                LocalTransformsNativeArray = nodeLocalTransforms,
                MovementDirection = movementDirection,
            }.Schedule(nodeComponents.Length, nodeComponents.Length / 4, state.Dependency);
            nodeMovementParallelJobHandle.Complete();
            
            nodeEntitiesQuery.CopyFromComponentDataArray(nodeComponents);
            nodeEntitiesQuery.CopyFromComponentDataArray(nodeLocalTransforms);

            nodeComponents.Dispose();
            nodeLocalTransforms.Dispose();
            
            return nodeMovementParallelJobHandle;
        }
        
        [BurstCompile]
        private JobHandle ScheduleWayMoveJob(ref SystemState state, float3 movementDirection)
        {
            var wayComponents = wayEntitiesQuery.ToComponentDataArray<WayComponent>(Allocator.TempJob);
            
            var wayMovementParallelJobHandle = new WayMovementParallelJob
            {
                WayComponentsNativeArray = wayComponents,
                MovementDirection = movementDirection,
            }.Schedule(wayComponents.Length, wayComponents.Length / 4, state.Dependency);
            wayMovementParallelJobHandle.Complete();
            
            wayEntitiesQuery.CopyFromComponentDataArray(wayComponents);
            wayComponents.Dispose();
            
            return wayMovementParallelJobHandle;
        }
        
        [BurstCompile]
        private JobHandle ScheduleLaneletMoveJob(ref SystemState state, float3 movementDirection)
        {
            var laneletComponents = laneletEntitiesQuery.ToComponentDataArray<LaneletComponent>(Allocator.TempJob);
            
            var laneletMovementParallelJobHandle = new LaneletMovementParallelJob
            {
                LaneletComponentsNativeArray = laneletComponents,
                MovementDirection = movementDirection,
            }.Schedule(laneletComponents.Length, laneletComponents.Length / 4, state.Dependency);
            laneletMovementParallelJobHandle.Complete();
            
            laneletEntitiesQuery.CopyFromComponentDataArray(laneletComponents);
            laneletComponents.Dispose();
            
            return laneletMovementParallelJobHandle;
        }
    }
}