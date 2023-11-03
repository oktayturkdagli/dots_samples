using Unity.Entities;

namespace MoveCurvit.Scripts.Components
{
    public struct InputComponent : IComponentData
    {
        public float AxisX;
        public float AxisY;
    }
}