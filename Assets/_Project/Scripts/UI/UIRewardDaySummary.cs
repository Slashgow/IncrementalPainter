using System;
using Coffee.UIExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityTimer;

public class UIRewardDaySummary : MonoBehaviour
{
    [SerializeField, Range(0f,2f)] private float timeBeforeAction = 1f;
    [SerializeField] private UIParticle particleReward;
    [SerializeField] private Image rankIconImage;
    [SerializeField] private Sprite sRankIcon, aRankIcon, bRankIcon, cRankIcon, dRankIcon;

    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private GameObject rewardLineSeparator;
    [SerializeField] private Color currencyColor, skillPointColor;

    [SerializeField] private UnityEvent OnAction;

    private Timer timerBeforeAction;
    private string currencyColorHex;
    private string skillPointHex;

    private void OnEnable()
    {
        currencyColorHex = ColorUtility.ToHtmlStringRGB(currencyColor);
        skillPointHex = ColorUtility.ToHtmlStringRGB(skillPointColor);
        LevelManager.OnEndLevel += LevelManager_OnEndLevel;
        LevelCompletionTracker.OnGiveReward += TryDisplayReward;
        HideReward();
    }

    private void OnDisable()
    {
        timerBeforeAction?.Cancel();
        LevelCompletionTracker.OnGiveReward -= TryDisplayReward;
        LevelManager.OnEndLevel -= LevelManager_OnEndLevel;
    }

    private void HideReward()
    {
        rankIconImage.gameObject.SetActive(false);
        particleReward.gameObject.SetActive(false);
        rewardText.gameObject.SetActive(false);
        rewardLineSeparator.SetActive(false);
    }

    private void LevelManager_OnEndLevel()
    {
        LevelData levelData = LevelManager.Instance.CurrentLevelData;
        LevelSaveData levelSaveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);
        TryDisplayBestRank(levelSaveData);

        timerBeforeAction = Timer.Register(timeBeforeAction, onComplete: () => OnAction?.Invoke(), useRealTime: true);

        rankIconImage.gameObject.SetActive(true);
    }

    private void TryDisplayReward(int skillPointEarned, int currencyEarned)
    {
        rewardText.gameObject.SetActive(true);
        rewardLineSeparator.SetActive(true);

        rewardText.text = $"{(currencyEarned > 0 ? $"<color=#{currencyColorHex}>+{FormatUtility.FormatValue(currencyEarned)} $</color>" : "")} " +
                          $"{(skillPointEarned > 0 ? $"<color=#{skillPointHex}>+{skillPointEarned} SP</color>" : "")}"; 
    }

    public void DisplayParticleEffect()
    {
        particleReward.gameObject.SetActive(true);
        particleReward.Play();
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
                    rankIconImage.color = LevelManager.Instance.GetColorForRank(LevelRank.S);
                    break;
                case LevelRank.A:
                    rankIconImage.sprite = aRankIcon;
                    rankIconImage.color = LevelManager.Instance.GetColorForRank(LevelRank.A);
                    break;
                case LevelRank.B:
                    rankIconImage.sprite = bRankIcon;
                    rankIconImage.color = LevelManager.Instance.GetColorForRank(LevelRank.B);
                    break;
                case LevelRank.C:
                    rankIconImage.sprite = cRankIcon;
                    rankIconImage.color = LevelManager.Instance.GetColorForRank(LevelRank.C);
                    break;
                case LevelRank.D:
                    rankIconImage.sprite = dRankIcon;
                    rankIconImage.color = LevelManager.Instance.GetColorForRank(LevelRank.D);
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
