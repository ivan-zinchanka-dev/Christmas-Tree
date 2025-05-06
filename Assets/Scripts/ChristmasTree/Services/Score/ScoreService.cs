using System;
using UnityEngine;

namespace ChristmasTree.Services.Score
{
    public class ScoreService
    {
        private const string BestScoreKey = "best_score";
        private int _bestScore;
        private int _score;

        public int Score => _score;
        public int BestScore => _bestScore;
        public BestScoreRecordInfo? RecordInfo { get; private set; }

        public event Action<int, int> OnScoreChanged;
        public event Action<BestScoreRecordInfo> OnNewBestScoreRecord;
        
        public ScoreService()
        {
            _bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        }

        public void Modify(int score)
        {
            int oldScore = _score;
            _score += score;

            if (_score > _bestScore)
            {
                int oldBestScore = _bestScore;
                _bestScore = _score;
                PlayerPrefs.SetInt(BestScoreKey, _bestScore);
                PlayerPrefs.Save();
                
                RecordInfo = new BestScoreRecordInfo(oldBestScore, _bestScore);
                OnNewBestScoreRecord?.Invoke(RecordInfo.Value);
            }
            
            OnScoreChanged?.Invoke(oldScore, _score);
        }

        public void Reset()
        {
            int oldScore = _score;
            _score = 0;
            RecordInfo = null;
            OnScoreChanged?.Invoke(oldScore, _score);
        }
    }
}