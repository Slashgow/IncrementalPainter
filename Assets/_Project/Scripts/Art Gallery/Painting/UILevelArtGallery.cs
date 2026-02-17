using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UILevelArtGallery : MonoBehaviour, IUISelectable<LevelData>, IPointerClickHandler
{
    [SerializeField] private Image drawingImage;

    public event Action<LevelData> OnSelectEvent;

    private LevelData levelData;
    public LevelData LevelData => levelData;
    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;

        drawingImage.sprite = levelData.LevelDrawing;
    }

    public LevelData GetSelectableData() => levelData;
    public void OnPointerClick(PointerEventData eventData) => OnSelect(levelData);
    public void OnSelect(LevelData data) =>  OnSelectEvent?.Invoke(levelData);
}
