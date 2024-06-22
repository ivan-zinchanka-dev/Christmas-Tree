using Services;

namespace Controls.Events
{
    internal struct OnProjectileDroppedComponent
    {
        public Projectile Projectile { get; private set; }

        public OnProjectileDroppedComponent(Projectile projectile)
        {
            Projectile = projectile;
        }
    }
}