﻿using Unity.Entities;

namespace LoadFromXML.Scripts.Component
{
    public struct PrefabComponent : IComponentData
    {
        public Entity NodePrefabEntity;
        public Entity LaneletPrefabEntity;
        public float NodeScale;
    }
}