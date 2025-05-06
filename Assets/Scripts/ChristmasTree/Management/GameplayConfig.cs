using System.Collections.Generic;
using UnityEngine;

namespace ChristmasTree.Management
{
    [CreateAssetMenu(fileName = "gameplay_config", menuName = "Configs/GameplayConfig", order = 0)]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField] 
        private float _gameDuration = 90.0f;
        [SerializeField] 
        private List<GameStage> _gameStages;

        public float GameDuration => _gameDuration;
        public IReadOnlyList<GameStage> GameStages => new List<GameStage>(_gameStages);
    }
}