using Leopotam.Ecs;
using Management;

namespace Controls.Projectiles
{
    public class ProjectileEventsSystem : IEcsRunSystem
    {
        private readonly EcsFilter<OnStarSpent> _starSpentFilter = null;
        private readonly GameStateMachine _gameStateMachine;
        
        public void Run()
        {
            if (_starSpentFilter.GetEntitiesCount() > 0)
            {
                _gameStateMachine.StarSpentNotify(_starSpentFilter.Get1(0).Success);
            }
        }
    }
}