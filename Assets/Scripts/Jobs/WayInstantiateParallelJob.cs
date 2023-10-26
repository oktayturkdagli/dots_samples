using ComponentAndTags;
using Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct WayInstantiateParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<WayData> WayDataNativeArray;
        [ReadOnly] public NativeArray<NodeData> NodeDataNativeArray;
        [ReadOnly] public NativeList<int> NodeListForWayNativeList;
        [ReadOnly] public int SortKey;
        public EntityCommandBuffer.ParallelWriter Ecb;
        
        public void Execute(int index)
        {
            var entity = Ecb.CreateEntity(SortKey);
            var wayData = WayDataNativeArray[index];
            
            Ecb.AddComponent(SortKey, entity, new WayComponent
            {
                ID = wayData.ID,
                Type = wayData.Type
            });
            
            var bufferComponent = Ecb.AddBuffer<NodeBufferComponent>(SortKey, entity);
            bufferComponent.Length = wayData.NodeCount;
            for (var i = 0; i < wayData.NodeCount; i++)
            {
                bufferComponent[i] = new NodeBufferComponent
                {
                    NodeData = NodeDataNativeArray[NodeListForWayNativeList[wayData.SliceStartId + i]]
                };
            }
        }
    }
}