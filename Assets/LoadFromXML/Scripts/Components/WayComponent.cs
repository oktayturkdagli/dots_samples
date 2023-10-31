using LoadFromXML.Scripts.Data;
using Unity.Entities;

namespace LoadFromXML.Scripts.Components
{
    public struct WayComponent : IComponentData
    {
        public int ID;
        public WayDataTypes Type;
    }
}