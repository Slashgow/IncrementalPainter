using System;
using UnityEngine;
using UnityTimer;

public class AutoClicker : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private inkolorgames.Logger logger;
    [SerializeField, Range(0f,3f)] private float clickTimerInterval = 0.1f;
    public float ClickTimerInterval => clickTimerInterval;
    [SerializeField] private bool useRealTime = false;
    public bool UseRealTime => useRealTime;

    public event Action<Vector3> OnClick;
    public event Action OnAutoClickStarted;
    public event Action OnAutoClickFinished;
    private Timer clickTimer;

    private void Start()
    {
        StartAutoClicking();
    }

    private void OnDestroy()
    {
        StopAutoClicking();
    }

    public void StartAutoClicking()
    {
        clickTimer?.Cancel();
        clickTimer = Timer.Register(clickTimerInterval, onComplete: SimulateClick, isLooped: true, useRealTime: useRealTime);
        OnAutoClickStarted?.Invoke();
    }

    public void StopAutoClicking()
    {
        clickTimer?.Cancel();
        OnAutoClickFinished?.Invoke();
    }

    private void SimulateClick()
    {
        logger.Log($"AutoClicker: SimulateClick at {Input.mousePosition}", this);
        OnClick?.Invoke(targetCamera.ScreenToWorldPoint(Input.mousePosition));
    }
}
