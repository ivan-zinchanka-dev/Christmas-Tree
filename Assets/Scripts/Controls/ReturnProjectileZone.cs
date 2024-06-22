using System;
using Controls.Events;
using Leopotam.Ecs;
using Management;
using Services;
using UnityEngine;
using Utilities;

namespace Controls
{
    public class ReturnProjectileZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Projectile>(out Projectile projectile) && 
                projectile.State != ProjectileState.Failed)
            {
                EcsEventBusUtility.FireOneFrameComponent(new OnProjectileDroppedComponent(projectile));
            }
        }
    }
}