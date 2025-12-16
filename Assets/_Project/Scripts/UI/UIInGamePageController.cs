using System;
using UnityEngine;
public class UIInGamePageController : UIPageController
{
    [SerializeField] private UIPage paintStatePage, upgradeStatePage, dayResumePage;

    private void Awake() => GameManager.OnStartGameState += GameManager_OnStartGameState;
    private void OnDestroy() => GameManager.OnStartGameState -= GameManager_OnStartGameState;

    private void GameManager_OnStartGameState(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.PAINT:
                if (paintStatePage == null)
                {
                    Debug.LogWarning("paint state page null");
                    return;
                }
                    

                ShowPage(paintStatePage);
                break;
            case GameManager.GameState.UPGRADE:
                if (paintStatePage == null)
                {
                    Debug.LogWarning("upgrade page null");
                    return;
                }

                ShowPage(upgradeStatePage);
                break;
            case GameManager.GameState.GALLERY:
                break;
            case GameManager.GameState.DAY_SUMMARY:
                if (paintStatePage == null)
                {
                    Debug.LogWarning("day resume page null");
                    return;
                }
                   

                ShowPage(dayResumePage);
                break;
            default:
                break;
        }
    }
}
 