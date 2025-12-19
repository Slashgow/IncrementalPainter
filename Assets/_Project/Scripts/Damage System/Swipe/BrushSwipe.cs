using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

[RequireComponent(typeof(LineRenderer))]
public class BrushSwipe : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private BrushSwipeData data;
    private float damage;
    private float criticalDamageMultiplier;
    private float criticalDamageChance;
    private LayerMask damageableLayers;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private Vector3 controlPoint;

    private float currentProgress = 0f;
    private Timer animationTimer;
    private Timer damageCheckTimer;

    private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

    public static event Action<float, Vector3, bool> OnAnyBrushSwipeAttack;

    public void Initialize(Vector3 origin, Vector3 direction, BrushSwipeData swipeData, 
        float damage, float criticalDamageMultiplier, float criticalDamageChance, LayerMask layers)
    {
        data = swipeData;
        this.damage = damage;
        this.criticalDamageMultiplier = criticalDamageMultiplier;
        this.criticalDamageChance = criticalDamageChance;
        damageableLayers = layers;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = data.pathResolution;
        lineRenderer.startWidth = data.visualWidth;
        lineRenderer.endWidth = data.visualWidth;
        lineRenderer.startColor = data.brushColor;
        lineRenderer.endColor = data.brushColor;

        CalculatePathPoints(origin, direction);
        StartSwipeAnimation();
    }

    private void CalculatePathPoints(Vector3 origin, Vector3 direction)
    {
        startPoint = origin;
        endPoint = origin + direction.normalized * data.swipeLength;

        // Calculate control point for quadratic Bezier curve
        Vector3 midPoint = (startPoint + endPoint) / 2f;

        // Perpendicular direction for curve offset
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0f).normalized;

        // Add randomized curvature
        float randomCurvature = data.curvature + UnityEngine.Random.Range(-data.curvatureVariation, data.curvatureVariation);
        randomCurvature = Mathf.Clamp01(randomCurvature);

        float offset = data.swipeLength * randomCurvature;
        controlPoint = midPoint + perpendicular * offset;
    }

    private void StartSwipeAnimation()
    {
        currentProgress = 0f;

        animationTimer = Timer.Register(data.duration, onComplete: () => Destroy(gameObject),
            onUpdate: t => {
                currentProgress = t / data.duration;
                UpdateVisual();
            },
            useRealTime: false
        );

        damageCheckTimer = Timer.Register(data.damageCheckInterval, onComplete: CheckDamageAlongPath, isLooped: true, useRealTime: false);
    }

    private void UpdateVisual()
    {
        int visiblePoints = Mathf.CeilToInt(data.pathResolution * currentProgress);
        visiblePoints = Mathf.Max(2, visiblePoints); 

        lineRenderer.positionCount = visiblePoints;

        for (int i = 0; i < visiblePoints; i++)
        {
            float t = (float)i / (data.pathResolution - 1);
            Vector3 point = Helper.CalculateBezierPoint(t, startPoint, endPoint, controlPoint);
            lineRenderer.SetPosition(i, point);
        }
    }

    private void CheckDamageAlongPath()
    {
        Vector3 currentPosition = Helper.CalculateBezierPoint(currentProgress, startPoint, endPoint, controlPoint);

        Painter.Instance.PaintAt(currentPosition, data.brushColor);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(currentPosition.x, currentPosition.y),data.damageWidth / 2f,damageableLayers);

        foreach (var collider in colliders)
        {
            // Avoid damaging the same object multiple times
            if (damagedObjects.Contains(collider.gameObject))
                continue;

            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                float damageAmount = CalculateDamage(out bool isCritical);
                OnAnyBrushSwipeAttack?.Invoke(damageAmount, currentPosition, isCritical);
                damageable.TakeDamage(damageAmount);
                damagedObjects.Add(collider.gameObject);
            }
        }
    }

    private float CalculateDamage(out bool isCritical)
    {
        isCritical = LuckUtility.RollLuck(criticalDamageChance);
        return isCritical ? damage * criticalDamageMultiplier : damage;
    }


    private void OnDestroy()
    {
        animationTimer?.Cancel();
        damageCheckTimer?.Cancel();            
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || data == null)
            return;

        Gizmos.color = Color.red;
        Vector3 currentPos = Helper.CalculateBezierPoint(currentProgress, startPoint, endPoint, controlPoint);
        Gizmos.DrawWireSphere(currentPos, data.damageWidth / 2f);

        Gizmos.color = Color.yellow;
        for (int i = 0; i < data.pathResolution - 1; i++)
        {
            float t1 = (float)i / (data.pathResolution - 1);
            float t2 = (float)(i + 1) / (data.pathResolution - 1);
            Gizmos.DrawLine(Helper.CalculateBezierPoint(t1, startPoint, endPoint, controlPoint), 
                Helper.CalculateBezierPoint(t2, startPoint, endPoint, controlPoint));
        }
    }
}
