using System.Collections.Generic;
using CW.Common;
using PaintCore;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class UIPercentCompletionBar : MonoBehaviour
{
    [SerializeField] private LocalizedString beforePercentage, afterPercentage;
    [SerializeField] private TextMeshProUGUI percentCompletionText;

    private List<CwChangeCounter> counters = new();

    private void Start()
    {
        counters.Add(LevelManager.Instance.CurrentLevelInstance.ColorChangeCounter);

        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDestroy()
    {
        if(GameManager.HasInstance)
            GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    private async void GameManager_OnStartGameState(GameManager.GameState gameState)
    {
        if (gameState != GameManager.GameState.DAY_SUMMARY)
            return;

        var total = CwChangeCounter.GetTotal(counters);
        var count = CwChangeCounter.GetCount(counters);
        var percent = CwCommon.RatioToPercentage(CwHelper.Divide(count, total), 1);

#if UNITY_WEBGL
    // WebGL - use async
    string before = await beforePercentage.GetLocalizedStringAsync().Task;
    string after = await afterPercentage.GetLocalizedStringAsync().Task;
#else
        string before = beforePercentage.GetLocalizedString();
        string after = afterPercentage.GetLocalizedString();
#endif

        percentCompletionText.text = $"{before} {percent.ToString()}% {after}";
    }
}
