using LoadFromXML.Scripts.Data;
using Unity.Collections;
using Unity.Entities;

namespace LoadFromXML.Scripts.Components
{
    public struct LoadComponent : IComponentData
    {
        public NativeArray<NodeData> NodeDataNativeArray;
        public NativeArray<WayData> WayDataNativeArray;
        public NativeArray<LaneletData> LaneletDataNativeArray;
        public NativeList<int> NodeListForWayNativeList;
    }
}