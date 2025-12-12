using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UICountdown : MonoBehaviour
{
    [SerializeField] private TMP_Text textCountdown;
    [SerializeField, Range(0f, 20f)] private float threshold; 

    public UnityEvent OnTickLessThanThreshold;

    private void Awake()
    {
        PaintStateManager.OnStartPaintState += HandleStartPaintState;
    }

    private void OnDestroy()
    {
        if (PaintStateManager.HasInstance)
        {
            PaintStateManager.OnStartPaintState -= HandleStartPaintState;
        }           
    }

    private void UpdateCountdownText(float timeRemaining)
    {
        if (timeRemaining <= threshold)
            OnTickLessThanThreshold?.Invoke();

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        textCountdown.text = $"{minutes:00}:{seconds:00}";
    }

    private void HandleStartPaintState()
    {
        PaintStateManager.Instance.CountdownPaintState.OnCountdownTick -= UpdateCountdownText;
        PaintStateManager.Instance.CountdownPaintState.OnCountdownTick += UpdateCountdownText;
    }
}
