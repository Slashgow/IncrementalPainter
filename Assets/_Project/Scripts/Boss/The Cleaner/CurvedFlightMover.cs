using System;
using NaughtyAttributes;
using UnityEngine;
using UnityTimer;

public class CurvedFlightMover : MonoBehaviour
{
    [Header("Flight Settings")]
    [SerializeField, Range(0f,5f)] private float flightDuration = 2f;
    [SerializeField] private AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [Header("Bezier Curve Settings")]
    [SerializeField] private BezierCurveType curveType = BezierCurveType.Quadratic;
    [SerializeField, Range(0f, 6f)] private float curveHeight = 3f;
    [SerializeField, Tooltip("1 or -1 for arc direction"), Range(-1f, 1f)] private float curveDirection = 1f; 
    [SerializeField, Tooltip("For cubic curves"), Range(0f, 1f)] private float curveTension = 0.5f;

    [Header("Custom Control Points (Optional)")]
    [SerializeField] private bool useCustomControlPoints = false;
    [SerializeField, ShowIf("useCustomControlPoints")] private Transform customControlPoint1;
    [SerializeField, ShowIf("useCustomControlPoints")] private Transform customControlPoint2; // Only for cubic

    public enum BezierCurveType
    {
        Quadratic,
        Cubic
    }

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 controlPoint1;
    private Vector3 controlPoint2;
    private float elapsedTime;
    private bool isFlying;
    private Timer flightTimer;

    public bool IsFlying => isFlying;
    public event Action OnReachTarget;

    public void StartFlight(Vector3 target, float? customDuration = null, float? customHeight = null)
    {
        startPosition = transform.position;
        targetPosition = target;
        elapsedTime = 0f;
        isFlying = true;

        float height = customHeight ?? curveHeight;

        // Calculate control points
        if (useCustomControlPoints)
        {
            if (customControlPoint1 != null)
                controlPoint1 = customControlPoint1.position;
            if (customControlPoint2 != null && curveType == BezierCurveType.Cubic)
                controlPoint2 = customControlPoint2.position;
        }
        else
        {
            if (curveType == BezierCurveType.Quadratic)
            {
                controlPoint1 = BezierUtility.CalculateQuadraticControlPoint(startPosition, targetPosition, height, curveDirection);
            }
            else // Cubic
            {
                (controlPoint1, controlPoint2) = BezierUtility.CalculateCubicControlPoints(startPosition, targetPosition, height, curveTension);
            }
        }

        float duration = customDuration ?? flightDuration;

        flightTimer?.Cancel();
        flightTimer = Timer.Register(
            duration,
            onComplete: () =>
            {
                transform.position = targetPosition;
                isFlying = false;
                OnReachTarget?.Invoke();
            },
            onUpdate: (float secondsElapsed) =>
            {
                elapsedTime = secondsElapsed;
                UpdatePosition();
            },
            useRealTime: false
        );
    }

    private void UpdatePosition()
    {
        if (!isFlying)
            return;

        float normalizedTime = elapsedTime / flightTimer.duration;
        float speedFactor = speedCurve.Evaluate(normalizedTime);
        float t = Mathf.Clamp01(normalizedTime * speedFactor);

        // Calculate position on Bezier curve
        if (curveType == BezierCurveType.Quadratic)
        {
            transform.position = BezierUtility.QuadraticBezier(startPosition, controlPoint1, targetPosition, t);
        }
        else // Cubic
        {
            transform.position = BezierUtility.CubicBezier(startPosition, controlPoint1, controlPoint2, targetPosition, t);
        }
    }

    private void OnDestroy()
    {
        flightTimer?.Cancel();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !isFlying)
            return;

        // Draw Bezier curve path
        Gizmos.color = Color.yellow;
        Vector3 previousPoint = startPosition;

        int segments = 30;
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 point;

            if (curveType == BezierCurveType.Quadratic)
            {
                point = BezierUtility.QuadraticBezier(startPosition, controlPoint1, targetPosition, t);
            }
            else // Cubic
            {
                point = BezierUtility.CubicBezier(startPosition, controlPoint1, controlPoint2, targetPosition, t);
            }

            Gizmos.DrawLine(previousPoint, point);
            previousPoint = point;
        }

        // Draw control points
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(controlPoint1, 0.2f);
        Gizmos.DrawLine(startPosition, controlPoint1);

        if (curveType == BezierCurveType.Cubic)
        {
            Gizmos.DrawWireSphere(controlPoint2, 0.2f);
            Gizmos.DrawLine(controlPoint2, targetPosition);
        }
        else
        {
            Gizmos.DrawLine(controlPoint1, targetPosition);
        }

        // Draw start and end points
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPosition, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 0.3f);
    }
}