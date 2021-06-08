using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;

[RequireComponent(typeof(Slider))]
public class UIProgressBar : MonoBehaviour
{
    private Slider slider;
    public float IntitalValue = 0f;
    public float FillingSpeed = 0.5f;
    private float targetProgress = 0f;
    public static bool FirstLoadDone { get; private set; }
    public static bool Ready { get; private set; }

    
    private void Awake()
    {
        slider = gameObject.GetComponent<Slider>();
        slider.value = IntitalValue;

    }
    // Start is called before the first frame update
    void Start() {
        if (!Ready) {
            TaskManager.AddTaskUpdateListener(UpdateTargetProgress);
            UpdateTargetProgress();
        }
    }

    // Update is called once per frame
    void Update() {
        if (!FirstLoadDone) {
            UpdateTargetProgress();
        }
        if (slider.value < targetProgress) {
            slider.value += FillingSpeed * Time.deltaTime;
        } else if (slider.value > targetProgress) {
            slider.value -= FillingSpeed * Time.deltaTime;
        }
            
    }

    public void IncrementProgressBar(float newProgress)
    {
        targetProgress = slider.value + newProgress;
    }

    private void UpdateTargetProgress() {
        if(PlayerManager.LocalPlayer != null) {
            float value = PlayerManager.LocalPlayer.PercentageTasksComplete();
            targetProgress = value;
            FirstLoadDone = true;
        }
    }
}
