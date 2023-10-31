using Data;
using Unity.Entities;

namespace LoadFromXML.Scripts.Component
{
    public struct WayComponent : IComponentData
    {
        public int ID;
        public WayDataTypes Type;
    }
}