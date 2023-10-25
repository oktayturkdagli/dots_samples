using Data;
using Unity.Entities;

namespace ComponentAndTag
{
    public class WayComponent : IComponentData
    {
        public int ID;
        public WayDataTypes Type;
    }
}