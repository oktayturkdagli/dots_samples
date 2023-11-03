using System.Collections.Generic;

namespace MoveCurvit.Scripts.Data
{
    public static class DataHolder
    {
        public static Dictionary<uint, LineRendererDataHolder> WayToLineRendererDictionary;
        public static Dictionary<uint, LaneletDataHolder> LaneletToMeshDictionary;
    }
}