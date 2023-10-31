using System;

namespace LoadFromXML.Scripts.Data
{
    public struct LaneletData
    {
        public int ID;
        public LaneletDataTypes Type;
        public int SpeedLimit;
        public int LeftWayId;
        public int RightWayId;
        public int MiddleWayId;
    }
}

[Flags]
public enum LaneletDataTypes
{
    None = 0,
    //Lanelet Type
    TypeRoad = 1 << 0,
    TypeCrosswalk = 1 << 1,
    TypeBicycle = 1 << 2,
    //Turn Direction
    TurnStraight = 1 << 3,
    TurnLeft = 1 << 4,
    TurnRight = 1 << 5,
    //Reverse
    ReverseNone = 1 << 6,
    ReverseLeft = 1 << 7,
    ReverseRight = 1 << 8,
    //Speed Limit
    SpeedLimitTrue = 1 << 9
}