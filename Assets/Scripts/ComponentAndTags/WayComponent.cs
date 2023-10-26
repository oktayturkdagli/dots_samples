using Data;
using Unity.Entities;

namespace ComponentAndTags
{
    public struct WayComponent : IComponentData
    {
        public int ID;
        public WayDataTypes Type;
    }
}