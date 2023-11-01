using MoveElements.Scripts.Components;
using MoveElements.Scripts.Data;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MoveElements.Scripts.Mono
{
    public class ElementCreator : MonoBehaviour
    {
        private NativeArray<ElementData> _elementDataNativeArray;
        private const int ElementCount = 1000;
        
        private void OnDisable()
        {
            _elementDataNativeArray.Dispose();
        }

        [ContextMenu("Create Elements")]
        private void CreateElements()
        {
            CreateElementsData();
            SpawnEntities();
        }
        
        private void CreateElementsData()
        {
            _elementDataNativeArray = new NativeArray<ElementData>(ElementCount, Allocator.Persistent);
            for (var i = 0; i < ElementCount; i++)
            {
                _elementDataNativeArray[i] = new ElementData()
                {
                    ID = i,
                    Position = new Vector3(i, 0, 0),
                };
            }
        }
        
        private void SpawnEntities()
        {
            var elementsComponent = new AllElementsComponent
            {
                ElementsNativeArray = _elementDataNativeArray,
            };
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var emptyEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(emptyEntity, elementsComponent);
        }
    }
}