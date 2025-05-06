using System;
using TMPro;
using UnityEngine;

namespace JanZinch.Services.CountdownTimer
{
    public class CountdownTimerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerTextMesh;
        
        private CountdownTimer _countdownTimer;
        
        public CountdownTimerView Initialize(CountdownTimer countdownTimer)
        {
            _countdownTimer = countdownTimer;
            _countdownTimer.OnLeftTimeUpdate += OnLeftTimeUpdate;

            OnLeftTimeUpdate(_countdownTimer.LeftTime);
            return this;
        }

        private void OnLeftTimeUpdate(TimeSpan leftTime)
        {
            _timerTextMesh.SetText($"{leftTime.Minutes:D2}:{leftTime.Seconds:D2}");
        }
    }
}