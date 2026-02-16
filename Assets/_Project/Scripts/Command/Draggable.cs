using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Settings")]
    [SerializeField] private bool enableDrag = true;
    [SerializeField] private float dragSmoothness = 1f;
    [SerializeField] private bool constrainToBounds = false;
    [SerializeField] private Bounds dragBounds;

    private Camera mainCamera;
    private Vector3 dragStartPosition;
    private Vector3 dragOffset;
    private Vector3 initialPosition;
    private bool isDragging = false;

    private float dragThreshold = 0.01f;

    private void Awake()
    {
        mainCamera = Camera.main;
        initialPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (!enableDrag) 
            return;

        isDragging = true;
        dragStartPosition = transform.position;

        Vector3 worldPoint = GetWorldPosition(eventData);
        dragOffset = transform.position - worldPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (!enableDrag || !isDragging) return;

        Vector3 worldPoint = GetWorldPosition(eventData);
        Vector3 targetPosition = worldPoint + dragOffset;

        if (constrainToBounds)
        {
            targetPosition = ConstrainToBounds(targetPosition);
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, dragSmoothness);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (!enableDrag || !isDragging) 
            return;

        isDragging = false;

        Vector3 finalPosition = transform.position;
        float dragDistance = Vector3.Distance(dragStartPosition, finalPosition);

        if (dragDistance > dragThreshold)
        {
            MoveCommand moveCommand = new MoveCommand(transform, dragStartPosition, finalPosition);
            CommandHistory.Instance.ExecuteCommand(moveCommand);
        }
        else
        {
            transform.position = dragStartPosition;
        }
    }

    private Vector3 GetWorldPosition(PointerEventData eventData)
    {
        Vector3 screenPoint = eventData.position;
        screenPoint.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPoint);
    }

    private Vector3 ConstrainToBounds(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, dragBounds.min.x, dragBounds.max.x),
            Mathf.Clamp(position.y, dragBounds.min.y, dragBounds.max.y),
            position.z
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (constrainToBounds)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(dragBounds.center, dragBounds.size);
        }
    }
}