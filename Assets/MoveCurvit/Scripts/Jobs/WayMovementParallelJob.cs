using MoveCurvit.Scripts.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace MoveCurvit.Scripts.Jobs
{
    [BurstCompile]
    public struct WayMovementParallelJob : IJobParallelFor
    {
        public NativeArray<WayComponent> WayComponentsNativeArray;
        [ReadOnly] public float3 MovementDirection;
        
        public void Execute(int index)
        {
            
        }
    }
}