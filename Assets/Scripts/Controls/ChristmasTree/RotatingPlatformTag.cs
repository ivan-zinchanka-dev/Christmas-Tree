using UnityEngine;

namespace Controls.ChristmasTree
{
    public class RotatingPlatformTag : MonoBehaviour
    {
        [SerializeField] private Transform _caughtDecorationsParent;
        public Transform CaughtDecorationsParent => _caughtDecorationsParent;
    }
}