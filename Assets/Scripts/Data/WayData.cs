using System;

namespace Data
{
    public struct WayData
    {
        public int ID;
        public WayDataTypes Type;
        public int SliceStartId;
        public int SliceEndId;
        
        public int NodeCount => SliceEndId - SliceStartId + 1;
    }
    
    [Flags]
    public enum WayDataTypes
    {
        None = 0,
        Solid = 1 << 0,
        Dashed = 1 << 1,
        Bidirectional = 1 << 2
    }
}