using UnityEngine;
using System.Collections.Generic;
using System;

public class TimerManager : MonoBehaviour {

    private static TimerManager instance;
    public static TimerManager getInstance() {
        if(instance == null) {
            GameObject sceneObject = new GameObject("Timer Manager");
            instance = sceneObject.AddComponent<TimerManager>();
            instance.timers = new List<Timer>();
        }

        return instance;
    }

    List<Timer> timers;

    // private void Awake() {
    //     timers = new List<Timer>();
    // }

    private void Update() {
        for (int i = 0; i < timers.Count; i++) {
            Timer timer = timers[i];
            if (timer != null && timer.isRunning()) {
                timer.updateTime(Time.deltaTime);
            }
        }
    }

    public Timer CreateAndRegisterTimer(float maxTime, bool repeating, bool startNow, Action onAlarmCallback) {
        Timer newTimer = new Timer(maxTime, repeating, onAlarmCallback);
        if(startNow) {
            newTimer.run();
        }
        timers.Add(newTimer);
        return newTimer;
    }

    // public void UnregisterTimer(Timer timer) {
    //     timers.Remove(timer);
    // }
}