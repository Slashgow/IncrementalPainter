using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [SerializeField] private Transform paintCameraTransform;
    [SerializeField] private Transform skillTreeCameraTransform;

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
                MoveCameraToPosition(paintCameraTransform.position, paintCameraTransform.rotation);
                break;
            case GameManager.GameState.DAY_SUMMARY:
                break;
            case GameManager.GameState.UPGRADE:
                MoveCameraToPosition(skillTreeCameraTransform.position, skillTreeCameraTransform.rotation);
                break;
            case GameManager.GameState.GALLERY:
                break;
            default:
                break;
        }
    }

    public void MoveCameraToPosition(Vector3 newPosition, Quaternion rotation)
    {
        mainCamera.transform.position = newPosition;
        mainCamera.transform.rotation = rotation;
    }

}
