using Unity.Collections;
using Unity.Entities;

namespace MoveCurvit.Scripts.Components
{
    public struct CurvitPathComponent : IComponentData
    {
        public FixedString512Bytes CurvitLoadPath;
    }
}