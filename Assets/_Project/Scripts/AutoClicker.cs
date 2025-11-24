using System;
using UnityEngine;
using UnityTimer;

public class AutoClicker : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private inkolorgames.Logger logger;
    [SerializeField] private float clickTimerInterval = 0.1f;
    [SerializeField] private bool useRealTime = false;

    public event Action<Vector3> OnClick;
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
    }

    public void StopAutoClicking()
    {
        clickTimer?.Cancel();
    }

    private void SimulateClick()
    {
        OnClick?.Invoke(targetCamera.ScreenToWorldPoint(Input.mousePosition));
    }
}
