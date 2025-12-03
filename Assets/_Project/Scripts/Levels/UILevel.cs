using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour, IUISelectable<LevelData>
{
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textAuthor;
    [SerializeField] private Image levelDrawing;

    [SerializeField] private Selectable selectable;
    [SerializeField] private Image lockBackground;
    [SerializeField] private TextMeshProUGUI lockDescription;

    private UnlockableLevel unlockableLevel;
    public event Action<LevelData> OnSelectEvent;

    public void Initialize(UnlockableLevel unlockableLevel)
    {
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
        levelDrawing.sprite = unlockableLevel.LevelData.LevelDrawing;

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
}
