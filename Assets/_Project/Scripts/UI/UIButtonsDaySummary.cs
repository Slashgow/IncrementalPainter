using UnityEngine;
using UnityEngine.UI;

public class UIButtonsDaySummary : MonoBehaviour
{
    [SerializeField] private Button continueButton, mainMenuButton, nextLevelButton, retryButton;

    public void OnEnable()
    {
        LevelManager.OnEndLevel += ShowOnlyEndButtons;

        ShowOnlyContinueButton();
    }
    private void OnDisable() => LevelManager.OnEndLevel -= ShowOnlyEndButtons;

    private void ShowOnlyContinueButton()
    {
        continueButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
    }

    private void ShowOnlyEndButtons()
    {
        continueButton.gameObject.SetActive(false);
        mainMenuButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(LevelManager.Instance.IsNextLevelUnlocked);
    }
}