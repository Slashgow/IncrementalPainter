using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class MouseDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hoverColor, normalColor;

    [Header("Drag Settings")]
    [SerializeField, Range(0.01f, 0.5f)] private float smoothTime = 0.05f;

    public UnityEvent OnStartDragUnityEvent;
    public UnityEvent OnEndDragUnityEvent;
    public UnityEvent OnHoverStartUnityEvent;
    public UnityEvent OnHoverEndUnityEvent;

    private Camera mainCam;
    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;
    private Vector3 grabOffset = Vector3.zero;

    private void Awake()
    {
        mainCam = Camera.main;
        if (mainCam == null)
            Debug.LogError("MouseDraggable: No Main Camera found!", this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartDragging(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) 
            return;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = transform.position.z;

        Vector3 targetPosition = mouseWorldPos - grabOffset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void OnPointerUp(PointerEventData eventData) => StopDragging();
    public void OnEndDrag(PointerEventData eventData) => StopDragging();

    private void StartDragging(PointerEventData eventData)
    {
        OnStartDragUnityEvent?.Invoke();
        isDragging = true;
        velocity = Vector3.zero;
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(eventData.position);
        mouseWorldPos.z = transform.position.z;
        grabOffset = mouseWorldPos - transform.position;
        transform.position = mouseWorldPos - grabOffset;
    }

    private void StopDragging()
    {
        OnEndDragUnityEvent?.Invoke();
        isDragging = false;
        velocity = Vector3.zero;
        grabOffset = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverStartUnityEvent?.Invoke();
        spriteRenderer.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverEndUnityEvent?.Invoke();
        spriteRenderer.color = normalColor;
    }
}