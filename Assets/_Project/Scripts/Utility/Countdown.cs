using System;
using UnityEngine;
using UnityTimer;

public class Countdown
{
    public event Action<float> OnCountdownTick;
    public event Action OnCountdownComplete;

    public float TotalDuration { get; private set; }
    public float TimeRemaining { get; private set; }
    public float TickInterval { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }

    private Timer mainTimer;
    private Timer tickTimer;
    private float elapsedTime;
    private Action onCompleteCallback;

    public Countdown(float duration, float tickInterval = 1f, Action onCompleteCallback = null)
    {
        TotalDuration = duration;
        TimeRemaining = duration;
        TickInterval = tickInterval;
        IsRunning = false;
        IsPaused = false;
        elapsedTime = 0f;
        this.onCompleteCallback = onCompleteCallback;
    }

    public void Start(MonoBehaviour autoAttachTo = null)
    {
        if (IsRunning)
        {
            Debug.LogWarning("Countdown is already running!");
            return;
        }

        IsRunning = true;
        IsPaused = false;
        elapsedTime = 0f;
        TimeRemaining = TotalDuration;

        if (autoAttachTo != null)
            mainTimer = autoAttachTo.AttachTimer(TotalDuration,onComplete: HandleComplete, onUpdate: UpdateTimeRemaining);
        else
            mainTimer = Timer.Register(TotalDuration,onComplete: HandleComplete, onUpdate: UpdateTimeRemaining);

        if (autoAttachTo != null)
            tickTimer = autoAttachTo.AttachTimer(TickInterval,onComplete: HandleTick,isLooped: true);
        else
            tickTimer = Timer.Register(TickInterval,onComplete: HandleTick,isLooped: true);
    }

    public void Pause()
    {
        if (!IsRunning || IsPaused)
        {
            Debug.LogWarning("Cannot pause countdown - not running or already paused!");
            return;
        }

        mainTimer?.Pause();
        tickTimer?.Pause();
        IsPaused = true;
    }

    public void Resume()
    {
        if (!IsRunning || !IsPaused)
        {
            Debug.LogWarning("Cannot resume countdown - not paused!");
            return;
        }

        mainTimer?.Resume();
        tickTimer?.Resume();
        IsPaused = false;
    }

    public void Cancel()
    {
        if (!IsRunning)
        {
            return;
        }

        Timer.Cancel(mainTimer);
        Timer.Cancel(tickTimer);

        IsRunning = false;
        IsPaused = false;
        mainTimer = null;
        tickTimer = null;
    }

    public void ChangeTickInterval(float newTickInterval)
    {
        TickInterval = newTickInterval;

        if (IsRunning)
        {
            Timer.Cancel(tickTimer);

            bool wasPaused = IsPaused;
            tickTimer = Timer.Register(TickInterval,onComplete: HandleTick,isLooped: true);

            if (wasPaused)
                tickTimer.Pause();
        }
    }

    public float GetProgressRatio() => elapsedTime / TotalDuration;

    private void UpdateTimeRemaining(float secondsElapsed)
    {
        elapsedTime = secondsElapsed;
        TimeRemaining = TotalDuration - secondsElapsed;
    }

    private void HandleTick() => OnCountdownTick?.Invoke(TimeRemaining);

    private void HandleComplete()
    {
        IsRunning = false;
        IsPaused = false;
        TimeRemaining = 0f;

        Timer.Cancel(tickTimer);
        tickTimer = null;
        mainTimer = null;

        OnCountdownComplete?.Invoke();
        onCompleteCallback?.Invoke();
    }
}
