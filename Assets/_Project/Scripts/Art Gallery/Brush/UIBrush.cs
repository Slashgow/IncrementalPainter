using System;
using UnityEngine;
using UnityEngine.UI;

public class UIBrush : MonoBehaviour, IUISelectable<BrushData>
{
    [SerializeField] private Image brushImagePreview;
    [SerializeField] private Button selectButton;

    private BrushData brushData;

    public event Action<BrushData> OnSelectEvent;

    public void Initialize(BrushData brushData)
    {
        this.brushData = brushData;

        brushImagePreview.sprite = brushData.BrushSprite;
    }

    private void OnEnable() => selectButton.onClick.AddListener(OnClickOnBrush);
    private void OnDisable() => selectButton.onClick.RemoveListener(OnClickOnBrush);
    private void OnClickOnBrush() => OnSelect(brushData);
    public BrushData GetSelectableData() => brushData;
    public void OnSelect(BrushData data) => OnSelectEvent?.Invoke(data);
}
