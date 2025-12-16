using UnityEngine;
using UnityEngine.UI;

public class UIGeneralSettings : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Awake() => mainMenuButton.onClick.AddListener(GoToMainMenu);
    private void OnDestroy() => mainMenuButton.onClick.RemoveListener(GoToMainMenu);

    private void GoToMainMenu() => SceneLoader.Instance.LoadSceneAsync(0);
}
