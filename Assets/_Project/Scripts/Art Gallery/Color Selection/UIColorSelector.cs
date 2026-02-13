using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIColorSelector : MonoBehaviour, IUISelectable<Color>, IPointerClickHandler
{
    [SerializeField] private Image image;

    private Color colorData;

    public event Action<Color> OnSelectEvent;

    public void Initialize(Color color)
    {
        colorData = color;
        image.color = color;
    }

    public Color GetSelectableData() => colorData;
    public void OnSelect(Color data) => OnSelectEvent?.Invoke(data);
    public void OnPointerClick(PointerEventData eventData) => OnSelect(colorData);
}
