using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuButton : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void OnEnable() => mainMenuButton.onClick.AddListener(GoToMainMenu);
    private void OnDisable() => mainMenuButton.onClick.RemoveListener(GoToMainMenu);

    private void GoToMainMenu() => SceneLoader.Instance.LoadSceneAsync(0);
}
