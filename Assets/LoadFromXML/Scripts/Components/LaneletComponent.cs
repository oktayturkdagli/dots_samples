using Unity.Entities;

namespace LoadFromXML.Scripts.Components
{
    public struct LaneletComponent : IComponentData
    {
        public int ID;
        public LaneletDataTypes Type;
        public int SpeedLimit;
        public int LeftWayId;
        public int RightWayId;
        public int MiddleWayId;
    }
}