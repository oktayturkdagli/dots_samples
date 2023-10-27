using ComponentAndTags;
using Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Jobs
{
    [BurstCompile]
    public struct LaneletInstantiateParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<LaneletData> LaneletDataNativeArray;
        [ReadOnly] public Entity LaneletEntity;
        [ReadOnly] public int SortKey;
        public EntityCommandBuffer.ParallelWriter Ecb;
        
        public void Execute(int index)
        {
            var entity = Ecb.Instantiate(SortKey, LaneletEntity);
            var laneletData = LaneletDataNativeArray[index];
            
            Ecb.AddComponent(SortKey, entity, new LaneletComponent
            {
                ID = laneletData.ID,
                Type = laneletData.Type,
                SpeedLimit = laneletData.SpeedLimit,
                LeftWayId = laneletData.LeftWayId,
                RightWayId = laneletData.RightWayId,
                MiddleWayId = laneletData.MiddleWayId
            });
        }
    }
}