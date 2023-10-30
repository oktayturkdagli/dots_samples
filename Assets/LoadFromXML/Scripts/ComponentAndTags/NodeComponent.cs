using Unity.Entities;
using Unity.Mathematics;

namespace ComponentAndTags
{
    public struct NodeComponent : IComponentData
    {
        public int ID;
        public float3 Position;
    }
}