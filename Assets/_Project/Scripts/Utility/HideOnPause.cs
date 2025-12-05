using UnityEngine;

public class HideOnPause : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    private void Start()
    {
        GameManager.OnPause += GameManager_OnPause;
        GameManager.OnResume += GameManager_OnResume;
    }

    private void OnDestroy()
    {
        if (GameManager.HasInstance)
        {
            GameManager.OnPause -= GameManager_OnPause;
            GameManager.OnResume -= GameManager_OnResume;
        }
    }

    private void GameManager_OnResume() => spriteRenderer.enabled = true;
    private void GameManager_OnPause() => spriteRenderer.enabled = false;
}
