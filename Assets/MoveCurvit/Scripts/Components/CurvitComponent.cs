using Unity.Entities;

namespace MoveCurvit.Scripts.Components
{
    public struct CurvitComponent : IComponentData
    {
        public Entity NodeEntity;
        public Entity WayEntity;
        public Entity LaneletEntity;
        public float NodeEntityScale;
    }
}