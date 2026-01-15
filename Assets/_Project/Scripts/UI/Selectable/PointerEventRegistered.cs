using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerEventRegistered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public event Action<PointerEventData> OnPointerClickEvent;
    public event Action<PointerEventData> OnPointerDownEvent;
    public event Action<PointerEventData> OnPointerEnterEvent;
    public event Action<PointerEventData> OnPointerExitEvent;
    public event Action<PointerEventData> OnPointerUpEvent;

    public void OnPointerClick(PointerEventData eventData) => OnPointerClickEvent?.Invoke(eventData);
    public void OnPointerDown(PointerEventData eventData) => OnPointerDownEvent?.Invoke(eventData);
    public void OnPointerEnter(PointerEventData eventData) => OnPointerEnterEvent?.Invoke(eventData);
    public void OnPointerExit(PointerEventData eventData) => OnPointerExitEvent?.Invoke(eventData);
    public void OnPointerUp(PointerEventData eventData) => OnPointerUpEvent?.Invoke(eventData);
}