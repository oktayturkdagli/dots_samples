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
            var ecbForNode = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var ecbForWay = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            var ecbForLanelet = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            
            var loadComponent = SystemAPI.GetSingleton<LoadComponent>();
            var loadComponentEntity = SystemAPI.GetSingletonEntity<LoadComponent>();
            var prefabComponent = SystemAPI.GetSingleton<PrefabComponent>();

            var nodeInstantiateParallelJobHandle = new NodeInstantiateParallelJob
            {
                SortKey = 0,
                Ecb = ecbForNode,
                NodeEntity = prefabComponent.NodePrefabEntity,
                NodeDataNativeArray = loadComponent.NodeDataNativeArray,
                NodeScale = prefabComponent.NodeScale
            }.Schedule(loadComponent.NodeDataNativeArray.Length, loadComponent.NodeDataNativeArray.Length / 4, state.Dependency);
            
            var wayInstantiateParallelJobHandle = new WayInstantiateParallelJob
            {
                SortKey = 0,
                Ecb = ecbForWay,
                WayDataNativeArray = loadComponent.WayDataNativeArray,
                NodeDataNativeArray = loadComponent.NodeDataNativeArray,
                NodeListForWayNativeList = loadComponent.NodeListForWayNativeList
            }.Schedule(loadComponent.WayDataNativeArray.Length, loadComponent.WayDataNativeArray.Length / 4, state.Dependency);
            
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
            
            state.Dependency = JobHandle.CombineDependencies(
                nodeInstantiateParallelJobHandle, 
                wayInstantiateParallelJobHandle, 
                laneletInstantiateParallelJobHandle);
            
            SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged)
                .DestroyEntity(loadComponentEntity);
        }
    }
}