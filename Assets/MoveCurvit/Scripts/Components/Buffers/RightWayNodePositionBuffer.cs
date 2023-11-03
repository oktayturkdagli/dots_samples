using Unity.Entities;
using Unity.Mathematics;

namespace MoveCurvit.Scripts.Components.Buffers
{
    public struct RightWayNodePositionBuffer: IBufferElementData
    {
        public float3 NodePosition;
    }
}