using UnityEngine;
using UnityEngine.UI;

public class UIButtonsDaySummary : MonoBehaviour
{
    [SerializeField] private Button upgradeButton, continueButton, startNextLevel;

    public void OnEnable()
    {
        LevelManager.OnEndLevel += LevelManager_OnEndLevel;
    }

    private void OnDisable()
    {
        if(LevelManager.HasInstance)
            LevelManager.OnEndLevel -= LevelManager_OnEndLevel;
    }

    private void LevelManager_OnEndLevel()
    {
        upgradeButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        startNextLevel.gameObject.SetActive(true);
    }
}