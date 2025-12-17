using System;
using Coffee.UIExtensions;
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

    [SerializeField] private UnityEvent OnAction;

    private Timer timerBeforeAction;

    private void OnEnable()
    {
        LevelManager.OnEndLevel += LevelManager_OnEndLevel;

        HideReward();
    }

    private void OnDisable()
    {
        timerBeforeAction?.Cancel();
        LevelManager.OnEndLevel -= LevelManager_OnEndLevel;
    }

    private void HideReward()
    {
        rankIconImage.gameObject.SetActive(false);
        particleReward.gameObject.SetActive(false);
    }

    private void LevelManager_OnEndLevel()
    {
        LevelData levelData = LevelManager.Instance.CurrentLevelData;
        LevelSaveData levelSaveData = GameSaveManager.Instance.LoadLevelData(levelData.LevelAuthor, levelData.LevelTitle);
        TryDisplayBestRank(levelSaveData);

        timerBeforeAction = Timer.Register(timeBeforeAction, onComplete: () => OnAction?.Invoke(), useRealTime: true);

        rankIconImage.gameObject.SetActive(true);
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
