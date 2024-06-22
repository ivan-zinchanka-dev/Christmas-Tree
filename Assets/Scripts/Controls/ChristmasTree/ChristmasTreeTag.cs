using UnityEngine;

namespace Controls.ChristmasTree
{
    public class ChristmasTreeTag : MonoBehaviour
    {
        [SerializeField] private Transform _caughtDecorationsParent;
        public Transform CaughtDecorationsParent => _caughtDecorationsParent;
    }
}