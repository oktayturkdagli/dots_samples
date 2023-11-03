using Unity.Entities;

namespace MoveCurvit.Scripts.Components.Buffers
{
    [InternalBufferCapacity(8)]
    public struct LaneletBuffer : IBufferElementData
    {
        public Entity LaneletEntity;
    }
}