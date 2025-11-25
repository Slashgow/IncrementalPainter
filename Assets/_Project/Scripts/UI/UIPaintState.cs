using TMPro;
using UnityEngine;

public class UIPaintState : MonoBehaviour
{
    [SerializeField] private TMP_Text textCountdown;

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
