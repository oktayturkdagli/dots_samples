using System.Runtime.InteropServices;
using MoveElements.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace MoveElements.Scripts.Jobs
{
    [BurstCompile]
    [StructLayout(LayoutKind.Auto)]
    public partial struct ElementMovementParallelJob : IJobEntity
    {
        [ReadOnly] public float3 MovementDirection;

        private void Execute(ref ElementComponent elementComponent, ref LocalTransform localTransform)
        {
            elementComponent.Position += MovementDirection;
            localTransform.Position = elementComponent.Position;
        }
    }
}