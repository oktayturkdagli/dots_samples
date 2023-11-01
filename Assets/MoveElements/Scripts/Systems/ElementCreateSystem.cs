using MoveElements.Scripts.Components;
using MoveElements.Scripts.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace MoveElements.Scripts.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial struct ElementCreateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AllElementsComponent>();
            state.RequireForUpdate<ElementPrefabComponent>();
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
            
            var elementsComponent = SystemAPI.GetSingleton<AllElementsComponent>();
            var elementPrefabComponent = SystemAPI.GetSingleton<ElementPrefabComponent>(); 
            var elementPrefabComponentEntity = SystemAPI.GetSingletonEntity<ElementPrefabComponent>();

            ScheduleJobs(ref state, elementsComponent, elementPrefabComponent);
            DestroyEntity(ref state, elementPrefabComponentEntity);
        }

        [BurstCompile]
        private void ScheduleJobs(ref SystemState state, AllElementsComponent allElementsComponent, ElementPrefabComponent elementPrefabComponent)
        {
            var elementInstantiateParallelJobHandle = ScheduleElementJob(ref state, allElementsComponent, elementPrefabComponent);
            elementInstantiateParallelJobHandle.Complete();
        }
        
        [BurstCompile]
        private JobHandle ScheduleElementJob(ref SystemState state, AllElementsComponent allElementsComponent, ElementPrefabComponent elementPrefabComponent)
        {
            var ecbForElement = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            
            var elementInstantiateParallelJobHandle = new ElementInstantiateParallelJob
            {
                SortKey = 0,
                Ecb = ecbForElement,
                ElementEntity = elementPrefabComponent.ElementPrefabEntity,
                ElementDataNativeArray = allElementsComponent.ElementsNativeArray,
            }.Schedule(allElementsComponent.ElementsNativeArray.Length, allElementsComponent.ElementsNativeArray.Length / 4, state.Dependency);
            
            return elementInstantiateParallelJobHandle;
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