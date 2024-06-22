using System;
using Cysharp.Threading.Tasks;
using Services;
using Services.Inventory;
using Services.Score;
using UI;
using UnityEngine;

namespace Management
{
    public class GameplaySceneRoot : MonoBehaviour
    {
        [SerializeField] private ViewDistributor _viewDistributor;
        [SerializeField] private EcsStartup _ecsStartup;
        
        [Space]
        [SerializeField] private GameplayConfig _gameplayConfig;
        [SerializeField] private InventoryConfig _inventoryConfig;
        
        private GameStateMachine _gameStateMachine;
        private InventoryService _inventoryService;
        private ScoreService _scoreService;
        
        private void Awake()
        {
            _inventoryService = new InventoryService(_inventoryConfig);
            _scoreService = new ScoreService();
            _gameStateMachine = new GameStateMachine(_gameplayConfig, _scoreService);
            
            _ecsStartup.Initialize(_gameStateMachine, _inventoryService, _scoreService);
            InitializeViews();
        }

        private void InitializeViews()
        {
            _viewDistributor.GetView<ScoreView>().Initialize(_scoreService);
            _viewDistributor.GetView<CountdownTimerView>().Initialize(_gameStateMachine.CountdownTimer);
        }

        private void OnEnable()
        {
            _gameStateMachine.OnNewState += OnNewGameState;
        }

        private void OnNewGameState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Summarizing:
                    OnSummarizing();
                    break;
                
                case GameState.Preparing:
                    OnGameRestart();
                    break;
            }
        }

        private async void OnSummarizing()
        {
            ScoreView scoreView = _viewDistributor.GetView<ScoreView>();
            scoreView.ShowDifference = false;

            await scoreView.ShowResultsPanelAsync();
            await _gameStateMachine.ProcessTimeBonusAsync();
            await scoreView.ShowBestResultAsync();
                
            await _viewDistributor.GetView<ScreenHintsView>().ShowContinueHintAsync();
            _gameStateMachine.AllowRestart();
        }
        
        private async void OnGameRestart()
        {
            _inventoryService.Refresh();
            _scoreService.Reset();
            
            ScoreView scoreView = _viewDistributor.GetView<ScoreView>();
            scoreView.ShowDifference = true;
            scoreView.RefreshAsync().Forget();
            
            _viewDistributor.GetView<ScreenHintsView>().Refresh();
        }

        private void OnDisable()
        {
            _gameStateMachine.OnNewState -= OnNewGameState;
        }
    }
}