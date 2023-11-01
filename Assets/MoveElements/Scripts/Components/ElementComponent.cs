using Unity.Entities;
using Unity.Mathematics;

namespace MoveElements.Scripts.Components
{
    public struct ElementComponent : IComponentData
    {
        public int ID;
        public float3 Position;
    }
}