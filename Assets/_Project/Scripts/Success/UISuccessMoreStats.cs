using TMPro;
using UnityEngine;

public class UISuccessMoreStats : MonoBehaviour
{
    [SerializeField] private UIPageSuccess uIPageSuccess;
    [SerializeField] private TextMeshProUGUI successDescriptionText;

    private void Awake()
    {
        successDescriptionText.text = string.Empty;
    }

    private void OnEnable()
    {
        uIPageSuccess.OnSelectSuccess += UIPageSuccess_OnSelectSuccess;
    }

    private void OnDisable()
    {
        uIPageSuccess.OnSelectSuccess -= UIPageSuccess_OnSelectSuccess;
    }

    private void UIPageSuccess_OnSelectSuccess(SuccessData successData)
    {
        DisplaySuccessInfo(successData);
    }

    private void DisplaySuccessInfo(SuccessData successData)
    {
        if (successData == null)
            return;

#if UNITY_WEBGL
        successData.Description.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                successDescriptionText.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
        successDescriptionText.text = successData.Description.GetLocalizedString();
#endif


        TryDisplaySuccessStat(successData);
    }

    private void TryDisplaySuccessStat(SuccessData successData)
    {
        SuccessStatData successStatData = SuccessManager.Instance.SuccessStatData;
        string steamID = successData.SteamId;
        successDescriptionText.text += $"\n\n";

        if (steamID == "SUCCESS_UNLOCK_X_COLOR_PALETTE")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.countColorPaletteUnlocked)} / " +
                $"{FormatUtility.FormatValue(ColorThemeUnlockManager.Instance.GetTotalItemCount())}";
        }
        else if(steamID == "SUCCESS_UNLOCK_X_TAMPON")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.countTamponUnlocked)} / {FormatUtility.FormatValue(TamponUnlockManager.Instance.GetTotalItemCount())}";
        }
        else if(steamID == "SUCCESS_FINISH_ALL_RANK_S")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.paintRankSFinish)} / {FormatUtility.FormatValue(LevelManager.Instance.TotalLevelCount)}";
        }
        else if (steamID == "SUCCESS_BUY_X_UPGRADES")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.upgradeBought)} / {FormatUtility.FormatValue(successData.Value)}";
        }
        else if(steamID == "SUCCESS_DEAL_X_DAMAGE_ONE_HIT")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.bestDamageSingleHit)} / {FormatUtility.FormatValue(successData.Value)}";
        }
        else if(steamID == "SUCCESS_DO_X_DAMAGE_ONE_SESSION")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.bestDamageOneSession)} / {FormatUtility.FormatValue(successData.Value)}";
        }
        else if(steamID == "SUCCESS_GET_X_CURRENCY_ONE_SESSION")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.bestCurrencyGainedOneSession)} / {FormatUtility.FormatValue(successData.Value)}";
        }
        else if(steamID == "SUCCESS_START_WITH_X_AMOUNT_PAINT")
        {
            successDescriptionText.text += $"{FormatUtility.FormatValue(successStatData.bestNumberOfBlobAtSessionStart)} / {FormatUtility.FormatValue(successData.Value)}";
        }
    }
}