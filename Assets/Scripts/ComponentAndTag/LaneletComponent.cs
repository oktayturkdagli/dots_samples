using Unity.Entities;

namespace ComponentAndTag
{
    public class LaneletComponent : IComponentData
    {
        public int ID;
        public LaneletDataTypes Type;
        public int SpeedLimit;
        public int LeftWayId;
        public int RightWayId;
        public int MiddleWayId;
    }
}