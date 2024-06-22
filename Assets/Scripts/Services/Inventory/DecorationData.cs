using System;
using UnityEngine;

namespace Services.Inventory
{
    [Serializable]
    public struct DecorationData
    {
        [field:SerializeField] public DecorationType Type { get; private set; }
        [field:SerializeField] public int Amount { get; private set; }
        [field:SerializeField] public Projectile Prefab { get; private set; }
    }
}