using Unity.Entities;

namespace ComponentAndTag
{
    public struct PrefabComponent : IComponentData
    {
        public Entity NodePrefabEntity;
        public Entity LaneletPrefabEntity;
        public float NodeScale;
    }
}