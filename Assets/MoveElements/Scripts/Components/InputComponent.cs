using Unity.Entities;

namespace MoveElements.Scripts.Components
{
    public struct InputComponent : IComponentData
    {
        public float axisX;
        public float axisY;
    }
}