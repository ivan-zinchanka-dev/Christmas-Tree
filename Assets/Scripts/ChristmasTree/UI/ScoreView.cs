using ChristmasTree.Services.Score;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ChristmasTree.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreCounterText;
        [SerializeField]
        private TextMeshProUGUI _scoreDifferenceText;
        [SerializeField]
        private TextMeshProUGUI _bestScoreText;
        [SerializeField]
        private TextMeshProUGUI _newRecordText;
        [SerializeField]
        private Transform _resultPanel;
        
        [SerializeField]
        private Transform _scoreCounterTargetPoint;
        [SerializeField]
        private Transform _resultPanelTargetPoint;

        private const float ShowResultsPanelDuration = 1.0f;
        private const float ShowBestResultDuration = 0.5f;
        private const float RefreshDuration = 0.25f;
        
        private ScoreService _scoreService;

        private Vector3 _scoreCounterSourcePosition;
        private Vector3 _resultPanelSourcePosition;
        private Tween _showDifferenceTween;
        private Tween _showResultsPanelTween;
        private Sequence _showBestResultTween;
        private Tween _refreshTween;

        public bool ShowDifference { get; set; } = true;

        private void Awake()
        {
            _scoreCounterSourcePosition = _scoreCounterText.rectTransform.position;
            _resultPanelSourcePosition = _resultPanel.transform.position;
        }

        public ScoreView Initialize(ScoreService scoreService, bool showDifference = true)
        {
            _scoreService = scoreService;
            ShowDifference = showDifference;
            
            _scoreService.OnScoreChanged += OnScoreChanged;
            _scoreCounterText.SetText(_scoreService.Score.ToString());
            _scoreDifferenceText.alpha = 0.0f;
            _newRecordText.alpha = 0.0f;
            
            return this;
        }
        
        public async UniTask ShowResultsPanelAsync()
        {
            _scoreCounterText.alignment = TextAlignmentOptions.Center;

            if (!_showResultsPanelTween.IsActive())
            {
                _showResultsPanelTween = DOTween.Sequence()
                    .Append(_resultPanel.DOMove(_resultPanelTargetPoint.position, ShowResultsPanelDuration)
                        .SetEase(Ease.OutSine))
                    .Join(_scoreCounterText.rectTransform.DOMove(GetScoreCounterTargetPosition(), 
                            ShowResultsPanelDuration).SetEase(Ease.OutSine))
                    .SetUpdate(true)
                    .SetLink(gameObject);
            }
            
            await UniTask.WaitForSeconds(_showResultsPanelTween.Duration(), true);
        }

        public async UniTask ShowBestResultAsync()
        {
            _bestScoreText.SetText($"Best Score:\n{_scoreService.BestScore}");

            if (_showBestResultTween.IsActive())
            {
                return;
            }

            _showBestResultTween = DOTween.Sequence()
                .Append(_bestScoreText.DOFade(1.0f, ShowBestResultDuration)
                    .SetEase(Ease.Linear))
                .SetUpdate(true)
                .SetLink(gameObject);

            if (_scoreService.RecordInfo.HasValue)
            {
                _showBestResultTween
                    .Append(_newRecordText.DOFade(1.0f, ShowBestResultDuration).SetEase(Ease.Linear));
            }

            await UniTask.WaitForSeconds(_showBestResultTween.Duration(), true);
        }
        
        public async UniTask RefreshAsync()
        {
            _scoreCounterText.alignment = TextAlignmentOptions.Right;
            
            if (!_refreshTween.IsActive())
            {
                _refreshTween = DOTween.Sequence()
                    .Append(_resultPanel.DOMove(_resultPanelSourcePosition, RefreshDuration)
                        .SetEase(Ease.InSine))
                    .Join(_scoreCounterText.rectTransform.DOMove(_scoreCounterSourcePosition, RefreshDuration)
                        .SetEase(Ease.InSine))
                    .Join(_bestScoreText.DOFade(0.0f, RefreshDuration)
                        .SetEase(Ease.Linear))
                    .Join(_newRecordText.DOFade(0.0f, RefreshDuration)
                        .SetEase(Ease.Linear))
                    .SetUpdate(true)
                    .SetLink(gameObject);
            }
            
            await UniTask.WaitForSeconds(_refreshTween.Duration(), true);
        }
        
        private void OnScoreChanged(int oldValue, int newValue)
        {
            _scoreCounterText.SetText(newValue.ToString());

            if (_showDifferenceTween.IsActive())
            {
                _showDifferenceTween.Kill();
            }

            if (ShowDifference)
            {
                _scoreDifferenceText.SetText(GetSignedValue(newValue - oldValue));
            
                _showDifferenceTween = DOTween.Sequence()
                    .Append(_scoreDifferenceText.DOFade(1.0f, 0.15f).SetEase(Ease.Flash))
                    .AppendInterval(0.5f)
                    .Append(_scoreDifferenceText.DOFade(0.0f, 0.5f).SetEase(Ease.Linear))
                    .SetUpdate(true).SetLink(gameObject);
            }
        }

        private Vector3 GetScoreCounterTargetPosition()
        {
            Vector3 difference = _resultPanel.transform.position - _scoreCounterTargetPoint.position;
            return _resultPanelTargetPoint.position - difference;
        }
        
        private static string GetSignedValue(int value)
        {
            if (value > 0)
            {
                return "+" + value.ToString();
            }
            else
            {
                return value.ToString();
            }
        }
    }
}