﻿using UnityEngine;

namespace Controls.ChristmasTree
{
    public class StarCatcherTag : MonoBehaviour
    {
        [SerializeField] private ChristmasTreeTag _christmasTreeTag = null;
        [SerializeField] private Transform _point = null;

        public ChristmasTreeTag TreeTag => _christmasTreeTag;
        public Transform Point => _point;
    }
}