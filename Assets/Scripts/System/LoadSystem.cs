using ComponentAndTag;
using Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace System
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial struct LoadSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LoadComponent>();
            state.RequireForUpdate<PrefabComponent>();
            state.RequireForUpdate<BeginPresentationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<EndInitializationEntityCommandBufferSystem.Singleton>();
        }
        
        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefabComponent = SystemAPI.GetSingleton<PrefabComponent>();
            var loadComponent = SystemAPI.GetSingleton<LoadComponent>();
            var loadComponentEntity = SystemAPI.GetSingletonEntity<LoadComponent>();

            ScheduleJobs(ref state, loadComponent, prefabComponent);
            DestroyEntity(ref state, loadComponentEntity);
        }

        [BurstCompile]
        private void ScheduleJobs(ref SystemState state, LoadComponent loadComponent, PrefabComponent prefabComponent)
        {
            var nodeInstantiateParallelJobHandle = ScheduleNodeJob(ref state, loadComponent, prefabComponent);
            var wayInstantiateParallelJobHandle = ScheduleWayJob(ref state, loadComponent);
            var laneletInstantiateParallelJobHandle = ScheduleLaneletJob(ref state, loadComponent, prefabComponent);
            
            state.Dependency = JobHandle.CombineDependencies(
                nodeInstantiateParallelJobHandle, 
                wayInstantiateParallelJobHandle, 
                laneletInstantiateParallelJobHandle);
        }
        
        [BurstCompile]
        private JobHandle ScheduleNodeJob(ref SystemState state, LoadComponent loadComponent, PrefabComponent prefabComponent)
        {
            var ecbForNode = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            
            var nodeInstantiateParallelJobHandle = new NodeInstantiateParallelJob
            {
                SortKey = 0,
                Ecb = ecbForNode,
                NodeEntity = prefabComponent.NodePrefabEntity,
                NodeDataNativeArray = loadComponent.NodeDataNativeArray,
                NodeScale = prefabComponent.NodeScale
            }.Schedule(loadComponent.NodeDataNativeArray.Length, loadComponent.NodeDataNativeArray.Length / 4, state.Dependency);
            
            return nodeInstantiateParallelJobHandle;
        }
        
        [BurstCompile]
        private JobHandle ScheduleWayJob(ref SystemState state, LoadComponent loadComponent)
        {
            var ecbForWay = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            
            var wayInstantiateParallelJobHandle = new WayInstantiateParallelJob
            {
                SortKey = 0,
                Ecb = ecbForWay,
                WayDataNativeArray = loadComponent.WayDataNativeArray,
                NodeDataNativeArray = loadComponent.NodeDataNativeArray,
                NodeListForWayNativeList = loadComponent.NodeListForWayNativeList
            }.Schedule(loadComponent.WayDataNativeArray.Length, loadComponent.WayDataNativeArray.Length / 4, state.Dependency);
            
            return wayInstantiateParallelJobHandle;
        }
        
        [BurstCompile]
        private JobHandle ScheduleLaneletJob(ref SystemState state, LoadComponent loadComponent, PrefabComponent prefabComponent)
        {
            var ecbForLanelet = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            
            var laneletInstantiateParallelJobHandle = new LaneletInstantiateParallelJob
            {
                SortKey = 0,
                Ecb = ecbForLanelet,
                LaneletEntity = prefabComponent.LaneletPrefabEntity,
                NodeDataNativeArray = loadComponent.NodeDataNativeArray,
                WayDataNativeArray = loadComponent.WayDataNativeArray,
                LaneletDataNativeArray = loadComponent.LaneletDataNativeArray,
                NodeListForWayNativeList = loadComponent.NodeListForWayNativeList
            }.Schedule(loadComponent.LaneletDataNativeArray.Length, loadComponent.LaneletDataNativeArray.Length / 4, state.Dependency);
            
            return laneletInstantiateParallelJobHandle;
        }

        [BurstCompile]
        private void DestroyEntity(ref SystemState state, Entity entity)
        {
            SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .DestroyEntity(entity);
        }
    }
}