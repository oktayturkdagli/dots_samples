using MoveElements.Scripts.Components;
using Unity.Entities;
using UnityEngine;

namespace MoveElements.Scripts.Authors
{
    public class ElementAuthoring : MonoBehaviour
    {
        public GameObject elementPrefab;
    }
    
    public class ElementBaker : Baker<ElementAuthoring>
    {
        public override void Bake(ElementAuthoring authoring)
        {
            DependsOn(authoring.elementPrefab);
            if (authoring.elementPrefab == null)
            {
                Debug.Log("elementPrefab prefab reference is null!");
                return;
            }
            
            var elementPrefabEntity = GetEntity(authoring.elementPrefab, TransformUsageFlags.Dynamic);
            var emptyEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(emptyEntity, new ElementPrefabComponent
            {
                ElementPrefabEntity = elementPrefabEntity,
            });
        }
    }
}