using MoveCurvit.Scripts.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MoveCurvit.Scripts.Mono
{
    public class CurvitController : MonoBehaviour
    {
        public string curvitLoadPath;

        private void Awake()
        {
            LoadXML();
        }
        
        [ContextMenu("Load XML")]
        private void LoadXML()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new CurvitPathComponent
            {
                CurvitLoadPath = new FixedString512Bytes(curvitLoadPath)
            });
        }
    }
}