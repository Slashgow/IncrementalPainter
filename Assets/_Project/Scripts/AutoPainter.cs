using PaintCore;
using PaintIn2D;
using UnityEngine;
using UnityTimer;



public class AutoPainter : MonoBehaviour
{
    [SerializeField] private inkolorgames.Logger logger;

    [Header("Paint Settings")]
    [Tooltip("Time in seconds between each automatic paint")]
    [SerializeField] private float paintInterval = 0.1f;

    [Tooltip("The camera used for raycasting (leave null for main camera)")]
    [SerializeField] private Camera targetCamera;

    [Tooltip("Layers to paint on")]
    [SerializeField] private LayerMask paintLayers = -1;

    [Header("Paint Components")]
    [Tooltip("The paint decal component that defines how to paint")]
    [SerializeField] private CwPaintDecal2D paintDecal;

    [Tooltip("Use real time (unaffected by Time.timeScale)")]
    [SerializeField] private bool useRealTime = false;

    private Timer paintTimer;
    private CwHitCache hitCache = new CwHitCache();

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (paintDecal == null)
            paintDecal = GetComponent<CwPaintDecal2D>();

        StartPainting();
    }

    private void OnDestroy() => StopPainting();

    private void StartPainting()
    {
        paintTimer?.Cancel();
        paintTimer = Timer.Register(paintInterval, PaintAtMousePosition, isLooped: true, useRealTime: useRealTime);
    }

    private void StopPainting() => paintTimer?.Cancel();

    private void PaintAtMousePosition()
    {
        if (targetCamera == null || paintDecal == null)
            return;

        Vector3 mousePosition = Input.mousePosition;

        PaintAt(mousePosition);
    }

    private void PaintAt(Vector3 paintPosition)
    {
        Ray ray = targetCamera.ScreenPointToRay(paintPosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, paintLayers);

        if (hit.collider != null)
        {
            paintDecal.HandleHitPoint(false, 0, 1f, Random.Range(int.MinValue, int.MaxValue), hit.point, Quaternion.identity);
            logger.Log($"Auto-painted at: {hit.point}", this);
        }
    }

    public void SetPaintInterval(float interval)
    {
        paintInterval = Mathf.Max(0.01f, interval);
        if (paintTimer != null && enabled)
        {
            StartPainting();
        }
    }
}