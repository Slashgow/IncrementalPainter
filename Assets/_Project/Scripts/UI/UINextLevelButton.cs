using UnityEngine;
using UnityEngine.UI;

public class UINextLevelButton : MonoBehaviour
{
    [SerializeField] private Button nextLevelButton;
    private void OnEnable() => nextLevelButton.onClick.AddListener(GoToNextLevel);
    private void OnDisable() => nextLevelButton.onClick.RemoveListener(GoToNextLevel);
    private void GoToNextLevel() => GameManager.Instance.StartNextLevel();
}
