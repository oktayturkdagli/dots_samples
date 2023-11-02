using Unity.Entities;

namespace MoveOsm.Scripts.Components
{
    public struct InputComponent : IComponentData
    {
        public float AxisX;
        public float AxisY;
    }
}