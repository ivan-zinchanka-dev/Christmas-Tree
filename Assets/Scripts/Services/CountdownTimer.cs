using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class CountdownTimer
    {
        private TimeSpan _maxTime;
        private TimeSpan _leftTime;
        private CountdownTimerState _state;
        private CancellationTokenSource _timerCts;
        
        public TimeSpan LeftTime => _leftTime;
        public event Action<TimeSpan> OnLeftTimeUpdate;
        public event Action OnTimeIsOver;
        
        private enum CountdownTimerState
        {
            Paused = 0,
            Resumed = 1,
            TimeIsOver = 2,
        }
        
        public CountdownTimer(TimeSpan leftTime)
        {
            _maxTime = leftTime;
            _leftTime = leftTime;
            _state = CountdownTimerState.Paused;
        }

        public void Reset()
        {
            Stop();
            _leftTime = _maxTime;
            _state = CountdownTimerState.Paused;
        }

        public void Add(TimeSpan deltaTime) {

            _leftTime = _leftTime.Add(deltaTime);
            OnLeftTimeUpdate?.Invoke(_leftTime);
        }
        
        public void Subtract(TimeSpan deltaTime) {

            _leftTime = _leftTime.Subtract(deltaTime);
            OnLeftTimeUpdate?.Invoke(_leftTime);
        }
        
        public async UniTaskVoid StartAsync()
        {
            _state = CountdownTimerState.Resumed;

            _timerCts = new CancellationTokenSource();
            
            while (_state == CountdownTimerState.Resumed && !_timerCts.IsCancellationRequested)
            {
                _leftTime = _leftTime.Subtract(TimeSpan.FromSeconds(Time.fixedDeltaTime));
                OnLeftTimeUpdate?.Invoke(_leftTime);
                
                if (_leftTime.TotalMilliseconds <= 0)
                {
                    _state = CountdownTimerState.TimeIsOver;
                    OnTimeIsOver?.Invoke();
                }
                
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, _timerCts.Token);     // <=> UniTask.DelayFrame(1);
            }
        }

        public void Stop()
        {
            _timerCts?.Cancel();
        }

    }
}