using MoveCurvit.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace MoveCurvit.Scripts.Jobs
{
    [BurstCompile]
    public struct NodeMovementParallelJob : IJobParallelFor
    {
        public NativeArray<NodeComponent> NodeComponentsNativeArray;
        public NativeArray<LocalTransform> LocalTransformsNativeArray;
        [ReadOnly] public float3 MovementDirection;
        
        public void Execute(int index)
        {
            var nodeComponent = NodeComponentsNativeArray[index];
            var localTransform = LocalTransformsNativeArray[index];
            
            nodeComponent.Position += MovementDirection;
            localTransform.Position = nodeComponent.Position;
            
            NodeComponentsNativeArray[index] = nodeComponent;
            LocalTransformsNativeArray[index] = localTransform;
        }
    }
}