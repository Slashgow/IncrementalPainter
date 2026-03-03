using System;
using UnityEngine;
public class UIInGamePageController : UIPageController
{
    [SerializeField] private UIPage paintStatePage, upgradeStatePage, dayResumePage, bossIntroductionPage;

    private void Start()
    {
    }
    private void Awake() => GameManager.OnStartGameState += GameManager_OnStartGameState;
    private void OnDestroy() => GameManager.OnStartGameState -= GameManager_OnStartGameState;

    private void GameManager_OnStartGameState(GameManager.GameState state)
    {
        //Debug.Log($"UIInGamePageController - GameManager_OnStartGameState: {state}");
        switch (state)
        {
            case GameManager.GameState.BOSS_INTRODUCTION:
                if (bossIntroductionPage == null)
                {
                    Debug.LogWarning("Boss introduction page null");
                    return;
                }
                ShowPage(bossIntroductionPage);
                break;

            case GameManager.GameState.PAINT:
                if (paintStatePage == null)
                {
                    Debug.LogWarning("Paint state page null");
                    return;
                }
                ShowPage(paintStatePage);
                break;

            case GameManager.GameState.UPGRADE:
                if (upgradeStatePage == null)
                {
                    Debug.LogWarning("Upgrade page null");
                    return;
                }
                ShowPage(upgradeStatePage);
                break;

            case GameManager.GameState.GALLERY:
                break;

            case GameManager.GameState.DAY_SUMMARY:
                if (dayResumePage == null)
                {
                    Debug.LogWarning("Day resume page null");
                    return;
                }
                ShowPage(dayResumePage);
                break;

            default:
                break;
        }
    }
}
 