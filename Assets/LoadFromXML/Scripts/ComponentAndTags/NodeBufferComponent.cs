using Data;
using Unity.Entities;

namespace ComponentAndTags
{
    public struct NodeBufferComponent : IBufferElementData
    {
        public NodeData NodeData;
    }
}