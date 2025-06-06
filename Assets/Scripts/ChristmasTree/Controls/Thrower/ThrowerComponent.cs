﻿using System;
using UnityEngine;

namespace ChristmasTree.Controls.Thrower
{
    [Serializable]
    public struct ThrowerComponent
    {
        public Transform StartPoint;
        public Vector3 ForceMultiplier;
        public float Recharge;
        public float ReturningSpeed;
        public TrajectoryDrawerComponent TrajectoryDrawerComponent;
    }
}