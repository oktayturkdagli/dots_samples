using Data;
using Unity.Entities;

namespace ComponentAndTag
{
    public struct WayComponent : IComponentData
    {
        public int ID;
        public WayDataTypes Type;
    }
}