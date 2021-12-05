using UnityEngine;
using System.Collections.Generic;
using System;

public class TimerManager : MonoBehaviour {
    List<Timer> timers;

    private void Awake() {
        timers = new List<Timer>();
    }

    private void Update() {
        foreach (Timer timer in timers) {
            if (timer.isRunning()) {
                timer.updateTime(Time.deltaTime);
            }
        }
    }

    public Timer RegisterNewTimer(float maxTime, bool repeating, Action onAlarmCallback) {
        Timer newTimer = new Timer(maxTime, repeating, onAlarmCallback);
        timers.Add(newTimer);
        return newTimer;
    }

    // public void UnregisterTimer(Timer timer) {
    //     timers.Remove(timer);
    // }
}