using UnityEngine;
using UnityEngine.Localization;

public class DemoUnlockedCondition : UnlockCondition
{
    [SerializeField] private LocalizedString sentenceLocalizedString;
    public override bool IsMet() => false;
    public override string GetDescription()
    {
#if !UNITY_WEBGL
        return sentenceLocalizedString.GetLocalizedString();
#endif

#if UNITY_WEBGL

        sentenceLocalizedString.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                    return handle.Result;
            }
        };
#endif

    }
}