﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    [Serializable]
    public struct TrajectoryDrawerComponent
    {
        public LineRenderer LineRenderer;
        [Range(3, 30)] public int SegmentCount;
        public int PointsCount;
        
        [HideInInspector] public List<Vector3> Points;
    }
}