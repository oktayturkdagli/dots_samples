using Data;
using Unity.Entities;

namespace LoadFromXML.Scripts.ComponentAndTags
{
    public struct NodeBufferComponent : IBufferElementData
    {
        public NodeData NodeData;
    }
}