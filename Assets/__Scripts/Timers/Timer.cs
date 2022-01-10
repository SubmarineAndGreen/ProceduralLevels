using System;

public class Timer {
    private int id;
    private float time;
    private float maxTime;
    private bool running;
    private bool repeating;
    private event Action onAlarm;

    public Timer(float maxTime, bool repeating, Action onAlarmCallback) {
        this.maxTime = maxTime;
        this.repeating = repeating;
        onAlarm += onAlarmCallback;
        running = false;
    }



    public void updateTime(float dt) {
        if (!running) {
            return;
        }
        time += dt;
        if (time > maxTime) {
            if (repeating) {
                time -= maxTime;
            } else {
                running = false;
            }

            onAlarm();
        }
    }

    public void run() {
        running = true;
    }

    public void stop() {
        running = false;
    }

    public void resetTime() {
        time = 0;
    }

    public bool isRunning() {
        return running;
    }

    public float getTimeRunning() {
        return time;
    }
}