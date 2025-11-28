using System;
using UnityEngine;
public class UIInGamePageController : UIPageController
{
    [SerializeField] private UIPage paintStatePage, upgradeStatePage, dayResumePage;

    private void Awake()
    {
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.OnStartGameState -= GameManager_OnStartGameState;
        }
    }

    private void GameManager_OnStartGameState(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.PAINT:
                ShowPage(paintStatePage);
                break;
            case GameManager.GameState.UPGRADE:
                ShowPage(upgradeStatePage);
                break;
            case GameManager.GameState.GALLERY:
                break;
            case GameManager.GameState.DAY_SUMMARY:
                ShowPage(dayResumePage);
                break;
            default:
                break;
        }
    }
}
 