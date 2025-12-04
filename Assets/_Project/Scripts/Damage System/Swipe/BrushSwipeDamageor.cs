using UnityEngine;

public class BrushSwipeDamageor : BaseDamageor
{
    [Header("Brush Swipe Settings")]
    [SerializeField] private BrushSwipe brushSwipePrefab;
    [SerializeField] private BrushSwipeData swipeData;

    [Header("Skill Data")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> swipeLengthPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> swipeDurationPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> damageWidthPerLevel;

    public SkillDataPerLevelOfType<FunctionAffine> SwipeLengthPerLevel => swipeLengthPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SwipeDurationPerLevel => swipeDurationPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> DamageWidthPerLevel => damageWidthPerLevel;

    public float SwipeLength => swipeLengthPerLevel.GetCurrentLevelData();
    public float SwipeDuration => swipeDurationPerLevel.GetCurrentLevelData();
    public float DamageWidth => damageWidthPerLevel.GetCurrentLevelData();

    private void OnEnable() => PaintBlob.OnAnyPaintBrushSwipeDie += PaintBlob_OnAnyPaintBrushSwipeDie;
    private void OnDisable() => PaintBlob.OnAnyPaintBrushSwipeDie -= PaintBlob_OnAnyPaintBrushSwipeDie;
    private void PaintBlob_OnAnyPaintBrushSwipeDie(Vector3 deathWorldPosition) => SpawnBrushSwipe(deathWorldPosition);

    private void SpawnBrushSwipe(Vector3 origin)
    {
        if (brushSwipePrefab == null)
        {
            Debug.LogWarning("BrushSwipePrefab is not assigned!");
            return;
        }

        Vector3 randomDirection = Random.insideUnitCircle.normalized;

        BrushSwipeData currentSwipeData = new BrushSwipeData
        {
            swipeLength = SwipeLength,
            curvature = swipeData.curvature,
            curvatureVariation = swipeData.curvatureVariation,
            duration = SwipeDuration,
            damageWidth = DamageWidth,
            damageCheckInterval = swipeData.damageCheckInterval,
            visualWidth = swipeData.visualWidth,
            brushColor = swipeData.brushColor,
            pathResolution = swipeData.pathResolution
        };

        BrushSwipe swipe = Instantiate(brushSwipePrefab, origin, Quaternion.identity);
        swipe.Initialize(origin, randomDirection, currentSwipeData, Damage, damageableLayers);
    }
}