using Unity.Entities;

namespace MoveCurvit.Scripts.Components.Buffers
{
    [InternalBufferCapacity(4)]
    public struct WayBuffer : IBufferElementData
    {
        public Entity WayEntity;
    }
}