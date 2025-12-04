using UnityEngine;

[System.Serializable]
public class BrushSwipeData
{
    [Header("Path Configuration")]
    [Tooltip("Total length of the swipe in world units")]
    public float swipeLength = 5f;

    [Tooltip("How much the path curves (0 = straight, 1 = very curved)")]
    [Range(0f, 1f)]
    public float curvature = 0.5f;

    [Tooltip("Random variation in curvature")]
    [Range(0f, 1f)]
    public float curvatureVariation = 0.2f;

    [Header("Timing")]
    [Tooltip("Duration of the entire swipe animation")]
    public float duration = 0.5f;

    [Header("Damage")]
    [Tooltip("Width of the damage area along the swipe")]
    public float damageWidth = 1f;

    [Tooltip("How often to check for damage along the path (in seconds)")]
    public float damageCheckInterval = 0.05f;

    [Header("Visual")]
    [Tooltip("Visual width of the brush stroke")]
    public float visualWidth = 0.5f;

    [Tooltip("Color of the brush stroke")]
    public Color brushColor = Color.white;

    [Tooltip("Number of segments to use when drawing the curve")]
    public int pathResolution = 20;
}
