using System;
using ChristmasTree.Services.Projectiles;
using UnityEngine;

namespace ChristmasTree.Services.Inventory
{
    [Serializable]
    public struct DecorationData
    {
        [field:SerializeField] 
        public DecorationType Type { get; private set; }
        
        [field:SerializeField] 
        public int Amount { get; private set; }
        
        [field:SerializeField] 
        public Projectile Prefab { get; private set; }
    }
}