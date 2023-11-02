using MoveElements.Scripts.Components;
using MoveElements.Scripts.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace MoveElements.Scripts.Jobs
{
    [BurstCompile]
    public struct ElementCreationParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<ElementData> ElementDataNativeArray;
        [ReadOnly] public Entity ElementEntity;
        [ReadOnly] public int SortKey;
        public EntityCommandBuffer.ParallelWriter Ecb;
        
        public void Execute(int index)
        {
            var entity = Ecb.Instantiate(SortKey, ElementEntity);
            var elementData = ElementDataNativeArray[index];
            
            Ecb.AddComponent(SortKey, entity, new ElementComponent
            {
                ID = elementData.ID,
                Position = elementData.Position,
            });
            
            Ecb.SetComponent(SortKey, entity, new LocalTransform
            {
                Position = elementData.Position,
                Rotation = quaternion.identity,
                Scale = 1f
            });
        }
    }
}