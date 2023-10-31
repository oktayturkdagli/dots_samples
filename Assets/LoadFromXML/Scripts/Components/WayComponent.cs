using Data;
using Unity.Entities;

namespace LoadFromXML.Scripts.ComponentAndTags
{
    public struct WayComponent : IComponentData
    {
        public int ID;
        public WayDataTypes Type;
    }
}