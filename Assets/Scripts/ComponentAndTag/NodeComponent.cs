using Unity.Entities;
using Unity.Mathematics;

namespace ComponentAndTag
{
    public class NodeComponent : IComponentData
    {
        public int ID;
        public float3 Position;
    }
}