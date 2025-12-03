using System;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class SimpleStunnable : MonoBehaviour, IStunnable
{
    [SerializeField] private inkolorgames.Logger logger;

    [Header("Stun Settings")]
    [SerializeField] private bool canBeStunned = true;

    private Timer stunTimer;

    public bool IsStunned => stunTimer != null && !stunTimer.isDone;
    public float StunTimeRemaining => stunTimer != null ? stunTimer.GetTimeRemaining() : 0f;

    public UnityEvent OnStunAppliedUnityEvent;
    public UnityEvent OnStunEndedUnityEvent;

    public event Action<float> OnStunApplied;
    public event Action OnStunEnded;

    public static event Action<Vector3, float> OnAnyStunnableStunned;
    public static event Action<Vector3> OnAnyStunnableStunEnded;

    private IMovable movable;

    public void ApplyStun(float duration)
    {
        if (!canBeStunned || duration <= 0f)
            return;

        movable = GetComponent<IMovable>();
        if(movable == null)
        {
            logger.LogWarning("Could not find any movable on this object, cannot stop movement", this);
            return;
        }

        if (IsStunned)
        {
            float currentTimeRemaining = stunTimer.GetTimeRemaining();

            if (duration > currentTimeRemaining)
            {
                stunTimer?.Cancel();
                StartStunTimer(duration);
            }
        }
        else
        {
            this.movable.StopMovement();

            OnStunApplied?.Invoke(duration);
            OnStunAppliedUnityEvent?.Invoke();
            OnAnyStunnableStunned?.Invoke(transform.position, duration);

            StartStunTimer(duration);
        }
    }

    private void StartStunTimer(float duration)
    {
        stunTimer = Timer.Register(duration, OnStunEnd, useRealTime: false);
    }

    private void OnStunEnd()
    {
        OnStunEnded?.Invoke();
        OnStunEndedUnityEvent?.Invoke();
        OnAnyStunnableStunEnded?.Invoke(transform.position);
        stunTimer = null;

        if(movable != null)
            movable.StartMovement();
    }

    private void OnDisable() => stunTimer?.Cancel();
}