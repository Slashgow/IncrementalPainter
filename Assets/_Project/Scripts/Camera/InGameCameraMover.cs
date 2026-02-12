using System;
using UnityEngine;

public class InGameCameraMover : CameraMover
{
    [SerializeField] private Transform paintCameraTransform;
    [SerializeField] private Transform skillTreeCameraTransform;
    [SerializeField, Range(0f, 30f)] private float upgradeStateZoom = 10f;

    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.OnStartGameState += GameManager_OnStartGameState;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameManager.OnStartGameState -= GameManager_OnStartGameState;
    }

    private void GameManager_OnStartGameState(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.PAINT:
                LockMovement();
                MoveAndZoomToSameTime(0f, 0f, LevelManager.Instance.CurrentLevelData.CameraPaintZoom, paintCameraTransform.position);
                break;
            case GameManager.GameState.DAY_SUMMARY:
                break;
            case GameManager.GameState.UPGRADE:
                MoveAndZoomToSameTime(0f, 0f, upgradeStateZoom, skillTreeCameraTransform.position);
                UnlockMovement();
                break;
            case GameManager.GameState.GALLERY:
                break;
            default:
                break;
        }
    }
}
