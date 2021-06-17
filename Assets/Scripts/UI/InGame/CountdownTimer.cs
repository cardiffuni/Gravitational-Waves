using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Managers;
using System;
using System.Globalization;

public class CountdownTimer : MonoBehaviour {
    private TMP_Text countdownText;
    public TimeSpan TimeLeft { get => TeamManager.TimeRemaining; }
    
    // Start is called before the first frame update
    void Start() {
        countdownText = transform.GetComponentInChildren<TMP_Text>();
        StartCoroutine(TeamManager.Countdown());
    }

    // Update is called once per frame
    void Update() {
        string timeLeft = string.Format(CultureInfo.CurrentCulture, "{0}:{1}:{2}", TimeLeft.Hours, TimeLeft.Minutes, TimeLeft.Seconds);
        countdownText.text = timeLeft;
    }
}
