using UnityEngine;
using UnityEngine.EventSystems;

public class TransformGizmo : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum GizmoType
    {
        ScaleCorner,
        RotateHandle,
        Destroy
    }

    [Header("Gizmo Settings")]
    [SerializeField] private GizmoType gizmoType = GizmoType.ScaleCorner;
    [SerializeField] private SpriteRenderer gizmoRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color dragColor = Color.green;

    private Transform targetTransform;
    private TransformController controller;
    private Camera mainCamera;
    private bool isDragging = false;

    // Scale corner specific
    private Vector3 oppositeCorner;
    private Vector3 dragStartScale;

    // Rotation specific
    private float dragStartAngle;
    private Quaternion dragStartRotation;

    public GizmoType Type => gizmoType;

    private void Awake()
    {
        mainCamera = Camera.main;

        gizmoRenderer.color = normalColor;
    }

    public void Initialize(Transform target, TransformController transformController, GizmoType type)
    {
        targetTransform = target;
        controller = transformController;
        gizmoType = type;
    }

    public void SetOppositeCorner(Vector3 corner)
    {
        oppositeCorner = corner;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (targetTransform == null || controller == null) return;

        isDragging = true;
        UpdateColor(dragColor);

        if (gizmoType == GizmoType.ScaleCorner)
        {
            dragStartScale = targetTransform.localScale;
        }
        else if (gizmoType == GizmoType.RotateHandle)
        {
            Vector3 worldPoint = GetWorldPosition(eventData);
            Vector3 direction = worldPoint - targetTransform.position;
            dragStartAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            dragStartRotation = targetTransform.rotation;
        }

        controller.OnBeginTransform(gizmoType);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (!isDragging || targetTransform == null) 
            return;

        if (gizmoType == GizmoType.ScaleCorner)
            HandleScaleDrag(eventData);

        else if (gizmoType == GizmoType.RotateHandle)
            HandleRotateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (!isDragging) 
            return;

        isDragging = false;
        UpdateColor(normalColor);

        if (gizmoType == GizmoType.ScaleCorner)
            controller.OnEndScale(dragStartScale, targetTransform.localScale);

        else if (gizmoType == GizmoType.RotateHandle)
            controller.OnEndRotate(dragStartRotation, targetTransform.rotation);
    }

    private void HandleScaleDrag(PointerEventData eventData)
    {
        Vector3 worldPoint = GetWorldPosition(eventData);

        // Calculate distance from opposite corner to mouse
        Vector3 oppositeWorldPos = targetTransform.TransformPoint(oppositeCorner);
        float currentDistance = Vector3.Distance(oppositeWorldPos, worldPoint);

        // Calculate initial distance
        Vector3 thisCornerWorld = targetTransform.TransformPoint(transform.localPosition);
        float initialDistance = Vector3.Distance(oppositeWorldPos, thisCornerWorld);

        if (initialDistance > 0.01f)
        {
            float scaleFactor = currentDistance / initialDistance;

            // Apply constraints
            scaleFactor = Mathf.Clamp(scaleFactor, controller.MinScale, controller.MaxScale);

            Vector3 newScale = dragStartScale * scaleFactor;

            // Maintain aspect ratio if enabled
            if (controller.MaintainAspectRatio)
            {
                float uniformScale = (newScale.x + newScale.y) / 2f;
                newScale = new Vector3(uniformScale, uniformScale, newScale.z);
            }

            targetTransform.localScale = newScale;
        }
    }

    private void HandleRotateDrag(PointerEventData eventData)
    {
        Vector3 worldPoint = GetWorldPosition(eventData);
        Vector3 direction = worldPoint - targetTransform.position;
        float currentAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float angleDelta = currentAngle - dragStartAngle;

        // Snap to increments if enabled
        if (controller.SnapRotation)
        {
            angleDelta = Mathf.Round(angleDelta / controller.RotationSnapAngle) * controller.RotationSnapAngle;
        }

        targetTransform.rotation = dragStartRotation * Quaternion.Euler(0, 0, angleDelta);
    }

    private Vector3 GetWorldPosition(PointerEventData eventData)
    {
        Vector3 screenPoint = eventData.position;
        screenPoint.z = Mathf.Abs(mainCamera.transform.position.z - targetTransform.position.z);
        return mainCamera.ScreenToWorldPoint(screenPoint);
    }

    private void UpdateColor(Color color)
    {
        if (gizmoRenderer != null)
            gizmoRenderer.color = color;
    }

    public void SetVisibility(bool visible)
    {
        if (gizmoRenderer != null)
            gizmoRenderer.enabled = visible;

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = visible;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging)
            UpdateColor(hoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging)
            UpdateColor(normalColor);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Type != GizmoType.Destroy)
            return;

        CommandHistory.Instance.ExecuteCommand(new DestroyPaintingCommand(controller.gameObject, controller.GetComponent<Level>().LevelData));
    }
}
