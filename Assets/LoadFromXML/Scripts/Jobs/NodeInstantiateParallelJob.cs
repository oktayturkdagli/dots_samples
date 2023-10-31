using Data;
using LoadFromXML.Scripts.Authors;
using LoadFromXML.Scripts.Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace LoadFromXML.Scripts.Jobs
{
    [BurstCompile]
    public struct NodeInstantiateParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<NodeData> NodeDataNativeArray;
        [ReadOnly] public Entity NodeEntity;
        [ReadOnly] public int SortKey;
        [ReadOnly] public float NodeScale;
        public EntityCommandBuffer.ParallelWriter Ecb;
        
        public void Execute(int index)
        {
            var entity = Ecb.Instantiate(SortKey, NodeEntity);
            var nodeData = NodeDataNativeArray[index];
            
            Ecb.AddComponent(SortKey, entity, new NodeComponent
            {
                ID = nodeData.ID,
                Position = nodeData.Position,
            });
            
            Ecb.SetComponent(SortKey, entity, new LocalTransform
            {
                Position = nodeData.Position,
                Rotation = quaternion.identity,
                Scale = NodeScale
            });
        }
    }
}