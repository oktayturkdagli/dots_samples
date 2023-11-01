using MoveElements.Scripts.Data;
using Unity.Collections;
using Unity.Entities;

namespace MoveElements.Scripts.Components
{
    public struct AllElementsComponent : IComponentData
    {
        public NativeArray<ElementData> ElementsNativeArray;
    }
}