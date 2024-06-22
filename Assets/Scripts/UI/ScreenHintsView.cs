using Controls;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ScreenHintsView : MonoBehaviour
    {
        [SerializeField] private float _showHideDuration = 0.25f;
        [SerializeField] private TextMeshProUGUI _preparingHint;
        [SerializeField] private TextMeshProUGUI _continueHint;
        [SerializeField] private ThrowerEventsAdaptor _throwerEventsAdaptor;
        
        private Tween _preparingHintTween;
        private Tween _continueHintTween;
        
        public async UniTask ShowContinueHintAsync()
        {
            await DoFadeContinueHint(1.0f).ToUniTask();
        }
        
        public async UniTask HideContinueHintAsync()
        {
            await DoFadeContinueHint(0.0f).ToUniTask();
        }

        public async UniTask ShowPreparingHintAsync()
        {
            await DoFadePreparingHint(1.0f).ToUniTask();
        }
        
        public async UniTask HidePreparingHintAsync()
        {
            await DoFadePreparingHint(0.0f).ToUniTask();
        }
        
        private Tween DoFadeContinueHint(float endValue)
        {
            if (_continueHintTween.IsActive())
            {
                _continueHintTween.Kill();
            }

            return _continueHintTween = _continueHint.DOFade(endValue, _showHideDuration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        private Tween DoFadePreparingHint(float endValue)
        {
            if (_preparingHintTween.IsActive())
            {
                _preparingHintTween.Kill();
            }

            return _preparingHintTween = _preparingHint.DOFade(endValue, _showHideDuration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .SetLink(gameObject);
        }
        
        public void Refresh()
        {
            HideContinueHintAsync().Forget();
            ShowPreparingHintAsync().Forget();
            Awake();
        }

        private void Awake()
        {
            _throwerEventsAdaptor.OnMouseInput.AddListener(OnMouseInput);
        }

        private void Start()
        {
            ShowPreparingHintAsync().Forget();
        }

        private void OnMouseInput()
        {
            HidePreparingHintAsync().Forget();
            _throwerEventsAdaptor.OnMouseInput.RemoveListener(OnMouseInput);
        }
    }
}