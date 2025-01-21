using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace VIRDY.SDK
{
    public class Timer : MonoBehaviour 
    {
        private float timer = 0.0f;
        private bool canCount = false;
        public TextMeshProUGUI HoursText;
        public TextMeshProUGUI MinutesText;
        public TextMeshProUGUI SecondsText;
        public TextMeshProUGUI MillisecondsText;
        private int lastMinute;

        private void Start() 
        {
            UpdateTimerText();
            lastMinute = DateTime.Now.Minute;
        }

        private void Update() 
        {
            if (canCount)
            {
                timer += Time.deltaTime;
                UpdateTimerText();
            }

            if (DateTime.Now.Minute != lastMinute)
            {
                GetCurrentTime();
                lastMinute = DateTime.Now.Minute;
            }
        }

        private void UpdateTimerText() 
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timer);
            int milliseconds = (int)((timer % 1) * 100); 

            HoursText.SetText(timeSpan.Hours.ToString("00"));
            MinutesText.SetText(timeSpan.Minutes.ToString("00"));
            SecondsText.SetText(timeSpan.Seconds.ToString("00"));
            MillisecondsText.SetText(milliseconds.ToString("00"));
        }

        public void StartTimer() 
        {
            canCount = true;
        }

        public void StopTimer() 
        {
            canCount = false;
        }

        public void ResetTimer() 
        {
            timer = 0.0f;
            UpdateTimerText();
        }

        public void GetCurrentTime() 
        {        
            DateTime currentTime = DateTime.Now;
            HoursText.SetText(currentTime.Hour.ToString("00"));
            MinutesText.SetText(currentTime.Minute.ToString("00"));
            // 초와 밀리세컨드는 공백으로 설정
            SecondsText.SetText("");
            MillisecondsText.SetText("");
            lastMinute = currentTime.Minute;
        }
    }
}
