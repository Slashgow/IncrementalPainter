using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UILevel : MonoBehaviour, IUISelectable<LevelData>
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

    private void UpdateLevelInfo()
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

            textRewardLeftToGet.text = $"{rewardLeftLocalizedString.GetLocalizedString()} " +
                $"{(remainingCurrencyReward > 0 ? $"<color=#{currencyColorHex}>{FormatUtility.FormatValue(remainingCurrencyReward)} $</color>" : "")} & " +
                $"{(remainingSPReward > 0 ? $"<color=#{skillPointHex}>{remainingSPReward} SP</color>" : "")} " +
                $"{rewardRightLocalizedString.GetLocalizedString()}";
        } 
        
        levelDrawing.sprite = unlockableLevel.LevelData.LevelDrawing;

        LevelSaveData levelSaveData = GameSaveManager.Instance.LoadLevelData(unlockableLevel.LevelData.LevelAuthor, unlockableLevel.LevelData.LevelTitle);
        TryDisplayOnGoingInfo(levelSaveData);
        TryDisplayBestRank(levelSaveData);
        TryDisplayUnlockConditions();
    }

    private void TryDisplayOnGoingInfo(LevelSaveData levelSaveData)
    {
        if(!levelSaveData.isDone && levelSaveData.completionRatio > 0f)
        {
            onGoingParent.SetActive(true);
            onGoingText.text = $"{dayLocalizedString.GetLocalizedString()} {levelSaveData.levelStats.CurrentDay} \n " +
                $"{Mathf.RoundToInt(levelSaveData.completionRatio * 100f)}/" +
                $"{Mathf.RoundToInt(unlockableLevel.LevelData.PercentCompletionCondition * 100f)} %";
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
                    break;
                case LevelRank.A:
                    rankIconImage.sprite = aRankIcon;
                    break;
                case LevelRank.B:
                    rankIconImage.sprite = bRankIcon;
                    break;
                case LevelRank.C:
                    rankIconImage.sprite = cRankIcon;
                    break;
                case LevelRank.D:
                    rankIconImage.sprite = dRankIcon;
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
