using Data;
using Unity.Collections;
using Unity.Entities;

namespace ComponentAndTag
{
    public struct LoadComponent : IComponentData
    {
        public NativeArray<NodeData> NodeData;
        public NativeArray<WayData> WayData;
        public NativeArray<LaneletData> LaneletData;
        public NativeList<int> NodeListForWay;
    }
}