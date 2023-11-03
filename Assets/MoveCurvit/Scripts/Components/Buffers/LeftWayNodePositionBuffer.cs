using Unity.Entities;
using Unity.Mathematics;

namespace MoveCurvit.Scripts.Components.Buffers
{
    public struct LeftWayNodePositionBuffer : IBufferElementData
    {
        public float3 NodePosition;
    }
}