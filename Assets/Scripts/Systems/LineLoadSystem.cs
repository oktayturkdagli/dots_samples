using ComponentAndTag;
using Data;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(LoadSystem))]
    [BurstCompile]
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

            var linePrefab = Resources.Load<LineRenderer>("Prefabs/Line");
            var matLineArrowBidirectionalDashed = Resources.Load<Material>("Materials/Mat_Line_Arrow_Bidirectional_Dashed");
            var matLineArrowBidirectionalSolid = Resources.Load<Material>("Materials/Mat_Line_Arrow_Bidirectional_Solid");
            var matLineArrowCommonDashed = Resources.Load<Material>("Materials/Mat_Line_Arrow_Common_Dashed");
            var matLineArrowCommonSolid = Resources.Load<Material>("Materials/Mat_Line_Arrow_Common_Solid");

            foreach (var wayData in loadComponent.WayDataNativeArray)
            {
                var lineRenderer = Object.Instantiate(linePrefab);
                lineRenderer.positionCount = wayData.NodeCount;
                
                for (var j = 0; j < wayData.NodeCount; j++)
                {
                    lineRenderer.SetPosition(j,
                        loadComponent
                            .NodeDataNativeArray[loadComponent.NodeListForWayNativeList[wayData.SliceStartId + j] - 1]
                            .Position);
                }
                
                var isBidirectional = (wayData.Type & WayDataTypes.Bidirectional) == WayDataTypes.Bidirectional;
                var isDashed = (wayData.Type & WayDataTypes.Dashed) == WayDataTypes.Dashed;
                var color = isBidirectional ? Color.yellow : Color.white;
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                if (isBidirectional)
                    lineRenderer.sharedMaterial = isDashed ? matLineArrowBidirectionalDashed : matLineArrowBidirectionalSolid;
                else
                    lineRenderer.sharedMaterial = isDashed ? matLineArrowCommonDashed : matLineArrowCommonSolid;
            }
        }
    }
}