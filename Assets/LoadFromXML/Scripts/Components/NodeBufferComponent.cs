using Data;
using Unity.Entities;

namespace LoadFromXML.Scripts.Component
{
    public struct NodeBufferComponent : IBufferElementData
    {
        public NodeData NodeData;
    }
}