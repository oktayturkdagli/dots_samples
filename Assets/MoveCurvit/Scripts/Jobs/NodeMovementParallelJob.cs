using MoveCurvit.Scripts.Components;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MoveCurvit.Scripts.Jobs
{
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    public partial struct NodeMovementParallelJob : IJobEntity
    {
        [ReadOnly] public float3 MovementDirection;

        private void Execute(ref NodeComponent nodeComponent, ref LocalTransform localTransform)
        {
            nodeComponent.Position += MovementDirection;
            localTransform.Position = nodeComponent.Position;
        }
    }
}