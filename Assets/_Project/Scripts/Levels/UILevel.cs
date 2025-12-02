using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour, IUISelectable<LevelData>
{
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textAuthor;
    [SerializeField] private Image levelDrawing;

    private LevelData levelData;
    public event Action<LevelData> OnSelectEvent;

    public void Initialize(LevelData level)
    {
        levelData = level;
        UpdateLevelInfo();
    }

    public void OnSelect(LevelData data)
    {
        LevelManager.Instance.SetCurrentLevel(data);
        OnSelectEvent?.Invoke(data);
    }

    public LevelData GetSelectableData() => levelData;

    private void UpdateLevelInfo()
    {
        textTitle.text = levelData.LevelTitle;
        textAuthor.text = $"{levelData.LevelAuthor} - {levelData.LevelDate}";
        levelDrawing.sprite = levelData.LevelDrawing;
    }
}
