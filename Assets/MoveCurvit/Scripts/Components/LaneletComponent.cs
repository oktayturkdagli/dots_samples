using Unity.Entities;

namespace MoveCurvit.Scripts.Components
{
    public struct LaneletComponent : IComponentData
    {
        public uint ID;
        public Entity LeftWay;
        public Entity RightWay;
    }
}