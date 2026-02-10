using System;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UILevel : MonoBehaviour, IUISelectable<LevelData>, IColorChanger
{
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textAuthor;
    [SerializeField] private Image levelDrawing;

    [SerializeField] private Selectable selectable;

    [Header("Lock UI Elements")]
    [SerializeField] private Image lockBackground;
    [SerializeField] private TextMeshProUGUI lockDescription;

    [Header("Rank UI Elements")]
    [SerializeField] private Image rankIconImage;
    [SerializeField] private Sprite sRankIcon, aRankIcon, bRankIcon, cRankIcon, dRankIcon;

    [Header("On-Going UI Elements")]
    [SerializeField] private GameObject onGoingParent;
    [SerializeField] private TextMeshProUGUI onGoingText;
    [SerializeField] private LocalizedString dayLocalizedString;

    [Header("Reward Left UI Elements")]
    [SerializeField] private LocalizedString rewardLeftLocalizedString, rewardRightLocalizedString;
    [SerializeField] private TextMeshProUGUI textRewardLeftToGet;
    [SerializeField] private Color currencyColor, skillPointColor;

    private string currencyColorHex;
    private string skillPointHex;
    private int remainingCurrencyReward;
    private int remainingSPReward;

    private UnlockableLevel unlockableLevel;
    public event Action<LevelData> OnSelectEvent;

    private void OnEnable() => ThemeColorManager.OnThemeChanged += OnThemeChanged;
    private void OnDisable() => ThemeColorManager.OnThemeChanged -= OnThemeChanged;
    public void OnThemeChanged(ColorTheme newTheme) => UpdateColor();
    public void UpdateColor() => UpdateLevelInfo();
    public void Initialize(UnlockableLevel unlockableLevel)
    {
        currencyColorHex = ColorUtility.ToHtmlStringRGB(currencyColor);
        skillPointHex = ColorUtility.ToHtmlStringRGB(skillPointColor);

        this.unlockableLevel = unlockableLevel;
        UpdateLevelInfo();
    }

    public void OnSelect(LevelData data)
    {
        if (!unlockableLevel.IsUnlocked)
            return;

        LevelManager.Instance.SetCurrentLevel(data);
        OnSelectEvent?.Invoke(data);
    }

    public LevelData GetSelectableData() => unlockableLevel.LevelData;

    private async void UpdateLevelInfo()
    {
        textTitle.text = unlockableLevel.LevelData.LevelTitle;
        textAuthor.text = $"{unlockableLevel.LevelData.LevelAuthor} - {unlockableLevel.LevelData.LevelDate}";

        remainingCurrencyReward = LevelManager.Instance.GetRemainingCurrencyReward(unlockableLevel);
        remainingSPReward = LevelManager.Instance.GetRemainingSkillPointReward(unlockableLevel);

        if(remainingCurrencyReward <= 0 && remainingSPReward <= 0)
            textRewardLeftToGet.gameObject.SetActive(false);
        else
        {
            textRewardLeftToGet.gameObject.SetActive(true);

            await UpdateRewardText();
        }

        levelDrawing.sprite = unlockableLevel.LevelData.LevelDrawing;

        LevelSaveData levelSaveData = GameSaveManager.Instance.LoadLevelData(unlockableLevel.LevelData.LevelAuthor, unlockableLevel.LevelData.LevelTitle);
        TryDisplayOnGoingInfo(levelSaveData);
        TryDisplayBestRank(levelSaveData);
        TryDisplayUnlockConditions();
    }

    private async Task UpdateRewardText() // or async Task
    {
#if UNITY_WEBGL
        string rewardLeft = await rewardLeftLocalizedString.GetLocalizedStringAsync();
        string rewardRight = await rewardRightLocalizedString.GetLocalizedStringAsync();
#else
        string rewardLeft = rewardLeftLocalizedString.GetLocalizedString();
        string rewardRight = rewardRightLocalizedString.GetLocalizedString();
#endif

        textRewardLeftToGet.text = $"{rewardLeft} " +
            $"{(remainingCurrencyReward > 0 ? $"<color=#{currencyColorHex}>{FormatUtility.FormatValue(remainingCurrencyReward)} $</color>" : "")} & " +
            $"{(remainingSPReward > 0 ? $"<color=#{skillPointHex}>{remainingSPReward} SP</color>" : "")} " +
            $"{rewardRight}";
    }


    private void TryDisplayOnGoingInfo(LevelSaveData levelSaveData)
    {
        if(!levelSaveData.isDone && levelSaveData.completionRatio > 0f)
        {
            onGoingParent.SetActive(true);


#if !UNITY_WEBGL
            onGoingText.text = $"{dayLocalizedString.GetLocalizedString()} {levelSaveData.levelStats.CurrentDay} \n " +
            $"{Mathf.RoundToInt(levelSaveData.completionRatio * 100f)}/" +
            $"{Mathf.RoundToInt(unlockableLevel.LevelData.PercentCompletionCondition * 100f)} %";
#endif

#if UNITY_WEBGL

        dayLocalizedString.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                  onGoingText.text = $"{handle.Result} {levelSaveData.levelStats.CurrentDay} \n " +
            $"{Mathf.RoundToInt(levelSaveData.completionRatio * 100f)}/" +
            $"{Mathf.RoundToInt(unlockableLevel.LevelData.PercentCompletionCondition * 100f)} %";
            }
        };
#endif

        }
        else
        {
            onGoingParent.SetActive(false);
        }
    }

    private void TryDisplayUnlockConditions()
    {
        if (unlockableLevel.IsUnlocked)
        {
            selectable.interactable = true;
            lockBackground.gameObject.SetActive(false);
            lockDescription.gameObject.SetActive(false);
        }
        else
        {
            selectable.interactable = false;
            lockBackground.gameObject.SetActive(true);
            lockDescription.gameObject.SetActive(true);

            var conditionDescription = new StringBuilder();
            foreach (var condition in unlockableLevel.UnlockConditions)
            {
                conditionDescription.Append($"{condition.GetDescription()} \n");
            }
            lockDescription.text = conditionDescription.ToString();

        }
    }

    private void TryDisplayBestRank(LevelSaveData levelSaveData)
    {
        if (levelSaveData.bestRank != LevelRank.None)
        {
            rankIconImage.gameObject.SetActive(true);
            switch (levelSaveData.bestRank)
            {
                case LevelRank.S:
                    rankIconImage.sprite = sRankIcon;
                    rankIconImage.color = ThemeColorManager.Instance.GetColor(ColorId.RANK_S);
                    break;
                case LevelRank.A:
                    rankIconImage.sprite = aRankIcon;
                    rankIconImage.color = ThemeColorManager.Instance.GetColor(ColorId.RANK_A);
                    break;
                case LevelRank.B:
                    rankIconImage.sprite = bRankIcon;
                    rankIconImage.color = ThemeColorManager.Instance.GetColor(ColorId.RANK_B);
                    break;
                case LevelRank.C:
                    rankIconImage.sprite = cRankIcon;
                    rankIconImage.color = ThemeColorManager.Instance.GetColor(ColorId.RANK_C);
                    break;
                case LevelRank.D:
                    rankIconImage.sprite = dRankIcon;
                    rankIconImage.color = ThemeColorManager.Instance.GetColor(ColorId.RANK_D);
                    break;
                default:
                    rankIconImage.gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            rankIconImage.gameObject.SetActive(false);
        }
    }


}
