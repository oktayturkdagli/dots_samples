﻿using Unity.Entities;
using Unity.Mathematics;

namespace LoadFromXML.Scripts.Component
{
    public struct NodeComponent : IComponentData
    {
        public int ID;
        public float3 Position;
    }
}