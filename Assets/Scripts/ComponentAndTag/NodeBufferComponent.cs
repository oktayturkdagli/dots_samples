using Data;
using Unity.Entities;

namespace ComponentAndTag
{
    public struct NodeBufferComponent : IBufferElementData
    {
        public NodeData NodeData;
    }
}