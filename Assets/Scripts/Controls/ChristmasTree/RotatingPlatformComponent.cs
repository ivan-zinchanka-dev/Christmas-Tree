using System;
using UnityEngine;

namespace Controls.ChristmasTree
{
    [Serializable]
    public struct RotatingPlatformComponent
    {
        public Transform Transform; 
        public Vector3 DefaultRotationSpeed;
        [HideInInspector] public Vector3 CurrentRotationSpeed;
        [HideInInspector] public Vector3 TargetRotationSpeed;
    }
}