using Unity.Entities;

namespace ComponentAndTags
{
    public struct PrefabComponent : IComponentData
    {
        public Entity NodePrefabEntity;
        public Entity LaneletPrefabEntity;
        public float NodeScale;
    }
}