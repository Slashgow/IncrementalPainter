using System;
using PaintCore;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIColorChangeProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBarImage;

    private CwChangeCounter counter;
    public UnityEvent OnStartDaySummary;

    private void Start()
    {
        counter = LevelManager.Instance.CurrentLevel.ColorChangeCounter;
        counter.OnUpdated += ColorChangeCounter_OnUpdated;
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }
    private void OnDestroy()
    {
        if (counter != null)
            counter.OnUpdated -= ColorChangeCounter_OnUpdated;

        if (GameManager.HasInstance)
            GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    private void GameManager_OnStartGameState(GameManager.GameState state)
    {
        if (state != GameManager.GameState.DAY_SUMMARY)
            return;

        OnStartDaySummary?.Invoke();
    }
    private void ColorChangeCounter_OnUpdated() => progressBarImage.fillAmount = counter.Ratio;
}
