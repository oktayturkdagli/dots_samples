using LoadFromXML.Scripts.Data;
using Unity.Entities;

namespace LoadFromXML.Scripts.Components
{
    public struct NodeBufferComponent : IBufferElementData
    {
        public NodeData NodeData;
    }
}