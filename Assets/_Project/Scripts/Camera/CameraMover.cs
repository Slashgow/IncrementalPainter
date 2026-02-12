using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    [Header("Camera Movement Settings")]
    [SerializeField, Range(0f, 10f)] private float dragMoveSpeed = 5f;
    [SerializeField, Range(0f, 100f)] private float keyMoveSpeed = 5f;
    [SerializeField, Range(0f, 10f)] private float zoomSpeed = 2f;
    [SerializeField, Range(0f, 10f)] private float minZoom = 2f;
    [SerializeField, Range(0f, 50f)] private float maxZoom = 10f;
    [SerializeField, Range(0f, 0.5f)] private float smoothTime = 0.1f;
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -5f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 5f);
    [SerializeField, Range(0f, 2f)] private float recenterDuration = 0.5f;
    [SerializeField] private Ease recenterEasing;
    [SerializeField] private bool lockMovementOnAwake = true;

    [SerializeField] private Camera cam;

    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;
    private CameraInputHandler inputHandler;
    private float zoomVelocity;
    private Vector3 originPosition;
    private bool isRecentering;

    public bool IsDraggindEnable { get; set; }
    public bool IsZoomingEnable { get; set; }
    public bool IsMovingWithWASD { get; set; }
    public bool IsMovementLocked { get; set; }

    private void Awake()
    {
        inputHandler = GetComponent<CameraInputHandler>();
        originPosition = transform.position;
        IsDraggindEnable = true;
        IsZoomingEnable = true;
        IsMovingWithWASD = true;
        IsMovementLocked = false;

        if(lockMovementOnAwake)
            LockMovement();
        else
            UnlockMovement();
    }

    protected virtual void OnEnable() => inputHandler.OnRecenterCamera += InputHandler_OnRecenterCamera;
    protected virtual void OnDisable() => inputHandler.OnRecenterCamera -= InputHandler_OnRecenterCamera;

    private void Start()
    { 
        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;
    }

    public void ZoomInstantTo(float targetZoom) => this.cam.orthographicSize = targetZoom;
    public void LockMovement() => IsMovementLocked = true;
    public void UnlockMovement() => IsMovementLocked = false;
    private void InputHandler_OnRecenterCamera()
    {
        isRecentering = true;
        this.transform.DOMove(originPosition, recenterDuration).SetEase(recenterEasing).SetUpdate(true).OnComplete(() => {
            isRecentering = false;
            targetPosition = transform.position;
        });
    }

    void Update()
    {
        if (isRecentering)
            return;

        if (IsMovementLocked)
            return;

        if (IsDraggindEnable)
            HandleDragging();

        if (IsZoomingEnable)
            HandleZooming();

        if (IsMovingWithWASD)
            HandleWASDMovement();

        SmoothMovement();
        SmoothZooming();
    }
    private void HandleWASDMovement()
    {
        Vector2 moveInput = inputHandler.MoveInput;
        if (moveInput.magnitude > 0)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0) * keyMoveSpeed * Time.unscaledDeltaTime;
            targetPosition += moveDirection;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }
    }

    private void HandleDragging()
    {
        if (!inputHandler.IsDragging)
            return;

        Vector3 difference = inputHandler.DragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        targetPosition += difference * dragMoveSpeed * Time.unscaledDeltaTime;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
    }

    private void HandleZooming()
    {
        if (inputHandler.ZoomInput == 0f)
            return;

        targetZoom -= inputHandler.ZoomInput * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    private void SmoothZooming()
    {
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
    }

    private void SmoothMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
    }

    public void MoveCameraToPosition(Vector3 newPosition, Quaternion rotation)
    {
        mainCamera.transform.position = newPosition;
        mainCamera.transform.rotation = rotation;

        targetPosition = mainCamera.transform.position;
    }
    public void MoveAndZoomToSequentially(float timeToMove, float timeToZoom, float targetZoom, Vector3 targetPosition, Action onComplete)
    {
        isRecentering = true;
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(this.transform.DOMove(targetPosition, timeToMove)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .OnComplete(() => {
                this.targetPosition = transform.position;
            }));
        sequence.Append(this.cam.DOOrthoSize(targetZoom, timeToZoom)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .OnComplete(() => {
                this.targetZoom = targetZoom;
                isRecentering = false;
                onComplete?.Invoke();
            }));
    }
    public void MoveAndZoomToSameTime(float timeToMove, float timeToZoom, float targetZoom, Vector3 targetPosition, Action onCompleteMove = null, Action onCompleteZoom = null)
    {
        isRecentering = true;
        this.transform.DOMove(targetPosition, timeToMove)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .OnComplete(() => {
                this.targetPosition = transform.position;
                onCompleteMove?.Invoke();
            });
        this.cam.DOOrthoSize(targetZoom, timeToZoom)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .OnComplete(() => {
                this.targetZoom = targetZoom;
                isRecentering = false;
                onCompleteZoom?.Invoke();
            });
    }

}
