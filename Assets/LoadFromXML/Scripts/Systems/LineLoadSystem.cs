using System.Runtime.InteropServices;
using Data;
using LoadFromXML.Scripts.Authors;
using LoadFromXML.Scripts.Component;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace LoadFromXML.Scripts.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(LoadSystem))]
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    public partial struct LineLoadSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LoadComponent>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
        
        public void OnUpdate(ref SystemState state)
        {
            var loadComponent = SystemAPI.GetSingleton<LoadComponent>();
            
            var lineRendererPrefab = LoadResource<LineRenderer>("Prefabs/Line");
            var bidirectionalDashedMaterial = LoadResource<Material>("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
            var bidirectionalSolidMaterial = LoadResource<Material>("Materials/Mat_Line_Arrow_Bidirectional_Solid");
            var commonDashedMaterial = LoadResource<Material>("Materials/Mat_Line_Arrow_Common_Dashed");
            var commonSolidMaterial = LoadResource<Material>("Materials/Mat_Line_Arrow_Common_Solid");
            
            foreach (var wayData in loadComponent.WayDataNativeArray)
            {
                var lineRenderer = Object.Instantiate(lineRendererPrefab);
                SetLineRendererPositions(lineRenderer, loadComponent, wayData);
                
                var isBidirectional = (wayData.Type & WayDataTypes.Bidirectional) == WayDataTypes.Bidirectional;
                var isDashed = (wayData.Type & WayDataTypes.Dashed) == WayDataTypes.Dashed;
                var color = isBidirectional ? Color.yellow : Color.white;
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;
                
                if (isBidirectional)
                    lineRenderer.sharedMaterial = isDashed ? bidirectionalDashedMaterial : bidirectionalSolidMaterial;
                else
                    lineRenderer.sharedMaterial = isDashed ? commonDashedMaterial : commonSolidMaterial;
            }
        }

        private T LoadResource<T>(string resourceAddress) where T : Object
        {
            var loadedResource = Resources.Load<T>(resourceAddress);
            if (loadedResource == null)
            {
                Debug.LogError("Resource not found at address: " + resourceAddress);
            }
            return loadedResource;
        }

        private void SetLineRendererPositions(LineRenderer lineRenderer, LoadComponent loadComponent, WayData wayData)
        {
            lineRenderer.positionCount = wayData.NodeCount;
            for (var j = 0; j < wayData.NodeCount; j++)
            {
                var nodeIndex = loadComponent.NodeListForWayNativeList[wayData.SliceStartId + j] - 1;
                var position = loadComponent.NodeDataNativeArray[nodeIndex].Position;
                lineRenderer.SetPosition(j, position);
            }
        }
    }
}