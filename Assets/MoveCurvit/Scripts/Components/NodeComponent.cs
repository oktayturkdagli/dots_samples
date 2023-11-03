using Unity.Entities;
using Unity.Mathematics;

namespace MoveCurvit.Scripts.Components
{
    public struct NodeComponent : IComponentData
    {
        public uint ID;
        public float3 Position;
    }
}