using UnityEngine;
using UnityEngine.UI;

public class UIRetryLevelButton : MonoBehaviour
{
    [SerializeField] private Button retryLevelButton;
    private void OnEnable() => retryLevelButton.onClick.AddListener(RetryLevel);
    private void OnDisable() => retryLevelButton.onClick.RemoveListener(RetryLevel);
    private void RetryLevel() => SceneLoader.Instance.ReloadSceneAsync();
}