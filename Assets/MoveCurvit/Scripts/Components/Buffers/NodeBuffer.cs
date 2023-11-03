using Unity.Entities;

namespace MoveCurvit.Scripts.Components.Buffers
{
    [InternalBufferCapacity(8)]
    public struct NodeBuffer : IBufferElementData
    {
        public Entity NodeEntity;
    }
}