using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITampon : MonoBehaviour, IUISelectable<TamponData>, IPointerClickHandler
{
    [SerializeField] private Image imagePreview;

    private TamponData tamponData;

    public event Action<TamponData> OnSelectEvent;

    public void Initialize(TamponData tamponData)
    {
        this.tamponData = tamponData;

        imagePreview.sprite = tamponData.TamponSprite;
    }

    public TamponData GetSelectableData() => tamponData;
    public void OnSelect(TamponData data) => OnSelectEvent?.Invoke(data);
    public void OnPointerClick(PointerEventData eventData) => OnSelect(tamponData);
}
