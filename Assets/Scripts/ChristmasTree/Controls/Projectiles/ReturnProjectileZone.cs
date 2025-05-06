using ChristmasTree.Controls.Events;
using ChristmasTree.Services.Projectiles;
using ChristmasTree.Utilities;
using UnityEngine;

namespace ChristmasTree.Controls.Projectiles
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