using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class UICountdown : MonoBehaviour
{
    [SerializeField] private TMP_Text textCountdown;

    [Header("Time added")]
    [SerializeField] private TextMeshProUGUI timeAddedText;
    [SerializeField, Range(0f,5f)] private float showDurationTimeAdded;

    [Header("Threshold Visual Changes")]
    [SerializeField, Range(0f, 20f)] private float threshold;
    [SerializeField] private Color thresholdColor;
    [SerializeField] private Color normalColor;

    public UnityEvent OnTickLessThanThreshold;
    public UnityEvent OnTimeAdded;
    private Timer timeAddedTimer;

    private void Awake()
    {
        timeAddedText.gameObject.SetActive(false);
        PaintStateManager.OnStartPaintState += HandleStartPaintState;
        PaintStateManager.OnAddedTimeToCountdown += HandleTimeAdded;
    }

    private void OnDestroy()
    {
        timeAddedTimer?.Cancel();

        if (PaintStateManager.HasInstance)
        {
            PaintStateManager.OnStartPaintState -= HandleStartPaintState;
            PaintStateManager.OnAddedTimeToCountdown -= HandleTimeAdded;
        }           
    }

    private void UpdateCountdownText(float timeRemaining)
    {
        if (timeRemaining <= threshold)
        {
            if(textCountdown.color != thresholdColor)
                textCountdown.color = thresholdColor;

            OnTickLessThanThreshold?.Invoke();
        }
        else
        {
            if (textCountdown.color != normalColor)
                textCountdown.color = normalColor;
        }
       
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        textCountdown.text = $"{minutes:00}:{seconds:00}";
    }

    private void HandleTimeAdded(float timeAdded)
    {
        timeAddedTimer?.Cancel();
        timeAddedText.gameObject.SetActive(true);
        timeAddedText.text = $"+{timeAdded} s";
        OnTimeAdded?.Invoke();
        timeAddedTimer = Timer.Register(showDurationTimeAdded, onComplete: () => timeAddedText.gameObject.SetActive(false));
    }

    private void HandleStartPaintState()
    {
        PaintStateManager.Instance.CountdownPaintState.OnCountdownTick -= UpdateCountdownText;
        PaintStateManager.Instance.CountdownPaintState.OnCountdownTick += UpdateCountdownText;
    }
}
