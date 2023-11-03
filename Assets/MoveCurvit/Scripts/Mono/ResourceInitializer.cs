using MoveCurvit.Scripts.Components;
using MoveCurvit.Scripts.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

namespace MoveCurvit.Scripts.Mono
{
    public class ResourceInitializer : MonoBehaviour
    {
        public float nodeScale = 1;
        
        private void Awake()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var nodeEntity = CreateNodeEntity(entityManager);
            var wayEntity = CreateWayEntity(entityManager);
            var laneletEntity= CreateLaneletEntity(entityManager);
            
            var curvitEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(curvitEntity, new CurvitComponent
            {
                NodeEntity = nodeEntity,
                WayEntity = wayEntity,
                LaneletEntity= laneletEntity,
                NodeEntityScale = nodeScale
            });
        }
        
        Entity CreateNodeEntity(EntityManager entityManager)
        {
            //Rendering
            var description = new RenderMeshDescription(ShadowCastingMode.Off, false, MotionVectorGenerationMode.ForceNoMotion);
            var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
        
            //Resource
            var mesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            var material = Resources.Load<Material>("Materials/Mat_Node");
        
            var nodeEntity = entityManager.CreateEntity();
            
            RenderMeshUtility.AddComponents(
                nodeEntity,
                entityManager,
                in description,
                new RenderMeshArray(new[] { material }, new[] { mesh }),
                materialMeshInfo
            );
            
            entityManager.AddComponentData(nodeEntity, new LocalTransform
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Scale = nodeScale
                }
            );
        
            entityManager.AddComponent<Prefab>(nodeEntity);
            return nodeEntity;
        }
        
        Entity CreateWayEntity(EntityManager entityManager)
        {
            var wayEntityPrefab = entityManager.CreateEntity();

            entityManager.AddComponent<Prefab>(wayEntityPrefab);
            return wayEntityPrefab;
        }
        
        Entity CreateLaneletEntity(EntityManager entityManager)
        {
            //Rendering
            var description = new RenderMeshDescription(ShadowCastingMode.Off, false, MotionVectorGenerationMode.ForceNoMotion);
            var materialMeshInfo = MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0);
        
            //Resource
            var mesh = MeshExtensions.CreateSimpleQuad(1, 1, "lanelet_procedural");
            var material = Resources.Load<Material>("Materials/Mat_Lanelet");
        
            var laneletEntityPrefab = entityManager.CreateEntity();

            RenderMeshUtility.AddComponents(
                laneletEntityPrefab,
                entityManager,
                in description,
                new RenderMeshArray(new[] { material }, new[] { mesh }),
                materialMeshInfo
            );

            entityManager.AddComponentData(laneletEntityPrefab, new LocalTransform
                {
                    Position = float3.zero,
                    Rotation = quaternion.identity,
                    Scale = 1
                }
            );

            entityManager.AddComponent<Prefab>(laneletEntityPrefab);
            return laneletEntityPrefab;
        }
        
    }
}