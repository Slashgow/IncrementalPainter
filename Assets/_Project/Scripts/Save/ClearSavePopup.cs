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


#if !UNITY_WEBGL
        warningText.text = warningLocalizedString.GetLocalizedString();
#endif

#if UNITY_WEBGL

        warningLocalizedString.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                    warningText.text = handle.Result;
            }
        };
#endif

    }

    protected override void OnClickDoActionButton()
    {
        GameSaveManager.Instance.ClearAllSaves();
        SceneLoader.Instance.ReloadSceneAsync();
        this.gameObject.SetActive(false);
    }
}