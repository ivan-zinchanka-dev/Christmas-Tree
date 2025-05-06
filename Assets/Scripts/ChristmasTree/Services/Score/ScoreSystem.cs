using Leopotam.Ecs;

namespace ChristmasTree.Services.Score
{
    public class ScoreSystem : IEcsRunSystem
    {
        private readonly ScoreService _scoreService;
        private readonly EcsFilter<OnScoreChangedComponent> _scoreChangedFilter = null;
        
        public ScoreSystem(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }
        
        public void Run()
        {
            foreach (int i in _scoreChangedFilter)
            {
                _scoreService.Modify(_scoreChangedFilter.Get1(i).Value);
                break;
            }
        }
    }
}