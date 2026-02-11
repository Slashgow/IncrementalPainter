using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

public class DemoUnlockedCondition : UnlockCondition
{
    [SerializeField] private LocalizedString sentenceLocalizedString;
    public override bool IsMet() => false;

    public override string GetDescription() => GetDescriptionAsync().Result;

    private async Task<string> GetDescriptionAsync()
    {
#if UNITY_WEBGL
        return await sentenceLocalizedString.GetLocalizedStringAsync().Task;
#else
       return sentenceLocalizedString.GetLocalizedString();
#endif
    }
}