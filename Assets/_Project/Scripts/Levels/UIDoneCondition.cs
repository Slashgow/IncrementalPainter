using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class UIDoneCondition : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDoneCondition;
    [SerializeField] private LocalizedString localizedDoneCondition;

    private void Start()
    {
        InitializeText();
    }

    private void InitializeText()
    {
        float percentCondition = Mathf.RoundToInt(LevelManager.Instance.CurrentLevelData.PercentCompletionCondition * 100f);
#if !UNITY_WEBGL
  
        textDoneCondition.text = $"({localizedDoneCondition.GetLocalizedString()} {percentCondition}%)";
#else
        localizedDoneCondition.GetLocalizedStringAsync().Completed += (handle) =>
                {
                    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        textDoneCondition.text = $"({handle.Result} {percentCondition}%)";
                    }
                };
#endif
    }
}
