using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class ClearSavePopup : PopUp
{
    [SerializeField] private LocalizedString warningLocalizedString;
    [SerializeField] private TextMeshProUGUI warningText;

    protected override void OnEnable()
    {
        base.OnEnable();
        warningText.text = warningLocalizedString.GetLocalizedString();
    }

    protected override void OnClickDoActionButton()
    {
        GameSaveManager.Instance.ClearAllSaves();
        SceneLoader.Instance.ReloadSceneAsync();
        this.gameObject.SetActive(false);
    }
}