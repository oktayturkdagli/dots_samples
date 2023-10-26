using ComponentAndTags;
using Unity.Entities;
using UnityEngine;

namespace AuthoringAndBakings
{
    public class PrefabAuthoring : MonoBehaviour
    {
        public GameObject nodePrefab;
        public GameObject laneletPrefab;
        public float nodeScale;
    }
    
    public class PrefabBaker : Baker<PrefabAuthoring>
    {
        public override void Bake(PrefabAuthoring authoring)
        {
            DependsOn(authoring.nodePrefab);
            DependsOn(authoring.laneletPrefab);
            
            if (authoring.nodePrefab == null)
            {
                Debug.Log("Prefab References Are NULL");
                return;
            }
            
            var nodePrefabEntity = GetEntity(authoring.nodePrefab, TransformUsageFlags.Dynamic);
            var laneletPrefabEntity = GetEntity(authoring.laneletPrefab, TransformUsageFlags.Renderable);
            var prefabPropertyEntity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(prefabPropertyEntity, new PrefabComponent
            {
                NodePrefabEntity = nodePrefabEntity,
                LaneletPrefabEntity = laneletPrefabEntity,
                NodeScale = authoring.nodeScale
            });
        }
    }
}