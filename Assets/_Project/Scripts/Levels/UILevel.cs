using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevel : MonoBehaviour, IUISelectable<Level>
{
    [SerializeField] private TextMeshProUGUI textTitle;
    [SerializeField] private TextMeshProUGUI textAuthor;
    [SerializeField] private Image levelDrawing;

    private Level levelData;
    public event Action<Level> OnSelectEvent;

    public void Initialize(Level level)
    {
        levelData = level;
        UpdateLevelInfo();
    }

    public void OnSelect(Level data)
    {
        OnSelectEvent?.Invoke(data);
        LevelManager.Instance.CurrentLevel = data;
    }

    public Level GetSelectableData() => levelData;

    private void UpdateLevelInfo()
    {
        textTitle.text = levelData.LevelTitle;
        textAuthor.text = $"{levelData.LevelAuthor} - {levelData.LevelDate}";
        levelDrawing.sprite = levelData.LevelDrawing;
    }
}
