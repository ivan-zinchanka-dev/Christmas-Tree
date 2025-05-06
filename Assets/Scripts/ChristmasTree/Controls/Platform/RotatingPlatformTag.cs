using UnityEngine;

namespace ChristmasTree.Controls.Platform
{
    public class RotatingPlatformTag : MonoBehaviour
    {
        [SerializeField] 
        private Transform _caughtDecorationsParent;
        public Transform CaughtDecorationsParent => _caughtDecorationsParent;
    }
}