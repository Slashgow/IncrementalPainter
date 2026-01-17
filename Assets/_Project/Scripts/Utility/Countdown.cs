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
    private MonoBehaviour attachedMonoBehaviour;

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
        attachedMonoBehaviour = autoAttachTo;

        if (autoAttachTo != null)
            mainTimer = autoAttachTo.AttachTimer(TotalDuration, onComplete: HandleComplete, onUpdate: UpdateTimeRemaining);
        else
            mainTimer = Timer.Register(TotalDuration, onComplete: HandleComplete, onUpdate: UpdateTimeRemaining);

        if (autoAttachTo != null)
            tickTimer = autoAttachTo.AttachTimer(TickInterval, onComplete: HandleTick, isLooped: true);
        else
            tickTimer = Timer.Register(TickInterval, onComplete: HandleTick, isLooped: true);
    }

    public void AddTime(float additionalTime)
    {
        if (!IsRunning)
        {
            Debug.LogWarning("Cannot add time - countdown is not running!");
            return;
        }

        if (additionalTime <= 0)
        {
            Debug.LogWarning("Additional time must be positive!");
            return;
        }

        // Update the total duration
        TotalDuration += additionalTime;
        TimeRemaining += additionalTime;

        // Store the current elapsed time before canceling
        float currentElapsed = elapsedTime;

        // Cancel and restart the main timer with the new duration
        bool wasPaused = IsPaused;
        float newDuration = TimeRemaining;

        Timer.Cancel(mainTimer);

        if (attachedMonoBehaviour != null)
            mainTimer = attachedMonoBehaviour.AttachTimer(newDuration, onComplete: HandleComplete, onUpdate: (secondsElapsed) => UpdateTimeRemainingWithOffset(secondsElapsed, currentElapsed));
        else
            mainTimer = Timer.Register(newDuration, onComplete: HandleComplete, onUpdate: (secondsElapsed) => UpdateTimeRemainingWithOffset(secondsElapsed, currentElapsed));

        if (wasPaused)
            mainTimer.Pause();
    }

    public void ReduceTime(float amount)
    {
        if (!IsRunning)
        {
            Debug.LogWarning("Cannot reduce time - countdown is not running!");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("Reduce amount must be positive!");
            return;
        }

        float actualReduce = Mathf.Min(amount, TimeRemaining);
        elapsedTime += actualReduce;
        TimeRemaining -= actualReduce;

        bool wasPaused = IsPaused;
        Timer.Cancel(mainTimer);

        if (TimeRemaining <= 0)
        {
            HandleComplete();
            return;
        }

        float offset = elapsedTime;
        float newDuration = TimeRemaining;

        if (attachedMonoBehaviour != null)
            mainTimer = attachedMonoBehaviour.AttachTimer(newDuration, onComplete: HandleComplete, onUpdate: (secondsElapsed) => UpdateTimeRemainingWithOffset(secondsElapsed, offset));
        else
            mainTimer = Timer.Register(newDuration, onComplete: HandleComplete, onUpdate: (secondsElapsed) => UpdateTimeRemainingWithOffset(secondsElapsed, offset));

        if (wasPaused)
            mainTimer.Pause();
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

            if (attachedMonoBehaviour != null)
                tickTimer = attachedMonoBehaviour.AttachTimer(TickInterval, onComplete: HandleTick, isLooped: true);
            else
                tickTimer = Timer.Register(TickInterval, onComplete: HandleTick, isLooped: true);

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

    private void UpdateTimeRemainingWithOffset(float secondsElapsed, float offset)
    {
        elapsedTime = offset + secondsElapsed;
        TimeRemaining = TotalDuration - elapsedTime;
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