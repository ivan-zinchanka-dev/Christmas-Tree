using System;
using UnityEngine;

namespace ChristmasTree.Management
{
    [Serializable]
    public struct GameStage
    {
        [field:SerializeField] 
        public int ActivationScore { get; private set; }
        [field:SerializeField] 
        public float MinPlatformSpeedMultiplier { get; private set; }
        [field:SerializeField] 
        public float MaxPlatformSpeedMultiplier { get; private set; }

        public bool IsDefault => ActivationScore <= 0;
    }
}