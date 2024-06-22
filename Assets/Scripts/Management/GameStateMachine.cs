using System;
using Cysharp.Threading.Tasks;
using Services;
using Services.Score;
using UnityEngine;

namespace Management
{
    public class GameStateMachine
    {
        private static readonly TimeSpan BonusCountingDelay = TimeSpan.FromSeconds(0.1f); 
        
        private int _activeStageIndex = 0;
        private readonly GameplayConfig _gameplayConfig;
        private readonly ScoreService _scoreService;
        
        public CountdownTimer CountdownTimer { get; private set; }
        
        public Action<GameStage> OnNewStage;
        public Action<GameState> OnNewState;
        
        public Action OnTimeBonusProcessStart;
        public Action OnTimeBonusProcessEnd;

        private GameState _activeState;

        public GameState ActiveState
        {
            get => _activeState;

            private set
            {
                _activeState = value;
                OnNewState?.Invoke(_activeState);
            }
        }
        
        public GameStateMachine(GameplayConfig gameplayConfig, ScoreService scoreService)
        {
            _gameplayConfig = gameplayConfig;
            _scoreService = scoreService;
            
            CountdownTimer = new CountdownTimer(TimeSpan.FromSeconds(_gameplayConfig.GameDuration));
            _scoreService.OnScoreChanged += OnScoreChanged;
            _activeState = GameState.Preparing;
        }

        public void Restart()
        {
            CountdownTimer.Reset();
            ActiveState = GameState.Preparing;
        }

        private void StartCountdown()
        {
            CountdownTimer.StartAsync().Forget();
            CountdownTimer.OnTimeIsOver += OnTimeIsOver;
        }

        public void Play()
        {
            StartCountdown();
            ActiveState = GameState.Playing;
        }

        public void AllowRestart()
        {
            ActiveState = GameState.WaitingForRestart;
        }

        public void StarSpentNotify(bool success) => StartSummarizing();
        private void OnTimeIsOver() => StartSummarizing();

        private void StartSummarizing()
        {
            if (ActiveState == GameState.Summarizing)
            {
                return;
            }
            
            CountdownTimer.Stop();
            ActiveState = GameState.Summarizing;
            ToDefaultStage();
        }

        public async UniTask ProcessTimeBonusAsync()
        {
            if (ActiveState != GameState.Summarizing)
            {
                Debug.LogWarning("Time bonus can't be proceeded");
                return;
            }

            OnTimeBonusProcessStart?.Invoke();
            
            while (CountdownTimer.LeftTime.TotalSeconds > 0.0f) {

                CountdownTimer.Subtract(TimeSpan.FromSeconds(1.0f));
                _scoreService.Modify(1);
                
                await UniTask.Delay(BonusCountingDelay, DelayType.DeltaTime);        
            }
            
            OnTimeBonusProcessEnd?.Invoke();
        }

        private void OnScoreChanged(int oldValue, int newValue)
        {
            if (_activeStageIndex >= _gameplayConfig.GameStages.Count - 1)
            {
                return;
            }

            if (newValue >= _gameplayConfig.GameStages[_activeStageIndex + 1].ActivationScore)
            {
                NextStage();
            }
        }

        private void NextStage()
        {
            if (ActiveState == GameState.Playing)
            {
                _activeStageIndex++;
                GameStage stage = _gameplayConfig.GameStages[_activeStageIndex];
                OnNewStage?.Invoke(stage);
            }
        }

        private void ToDefaultStage()
        {
            _activeStageIndex = 0;
            GameStage stage = _gameplayConfig.GameStages[_activeStageIndex];
            OnNewStage?.Invoke(stage);
        }


    }
}