using inkolorgames;
using PaintIn2D;
using UnityEngine;

[DisallowMultipleComponent]
public class Painter : MonoSingleton<Painter>
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> splatterScalePerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SplatterScalePerLevel => splatterScalePerLevel;
    public Vector3 SplatterScale => Vector3.one * splatterScalePerLevel.GetCurrentLevelData();

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> chancePercentageOfPaintingOnShieldBreakPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ChancePercentageOfPaintingOnShieldBreakPerLevel => chancePercentageOfPaintingOnShieldBreakPerLevel;
    public float ChancePercentageOfPaintingOnShieldBreak => chancePercentageOfPaintingOnShieldBreakPerLevel.GetCurrentLevelData();

    [SerializeField] private CwPaintDecal2D paintDecal;
    [SerializeField] private CwPaintDecal2D paintDecalShield;

    private void OnEnable()
    {
        SimpleDamageable.OnAnyDamageableDie += HandleOnDie;
        SimpleShieldable.OnAnyShieldBroken += HandleOnDieShield;
    }

    private void OnDisable()
    {
        SimpleDamageable.OnAnyDamageableDie -= HandleOnDie;
        SimpleShieldable.OnAnyShieldBroken -= HandleOnDieShield;
    }

    private void UpdateSplatterScale(float enemyScale) => paintDecal.Scale = SplatterScale * enemyScale;
    private void HandleOnDie(Vector3 worldPos, Color color, float scale)
    {
        UpdateSplatterScale(scale);
        PaintAt(worldPos, color);
    }

    public void PaintAt(Vector3 worldPos, Color color)
    {
        paintDecal.Color = color;
        paintDecal.HandleHitPoint(false, 0, 1f, Random.Range(int.MinValue, int.MaxValue), worldPos, Quaternion.identity);
    }

    private void UpdateSplatterScaleShield(float scale) => paintDecalShield.Scale = SplatterScale * scale;

    private void HandleOnDieShield(Vector3 worldPos, Color color, float scale)
    {
        if(!LuckUtility.RollLuck(ChancePercentageOfPaintingOnShieldBreak))
            return;

        UpdateSplatterScaleShield(scale);
        PaintShieldAt(worldPos, color);
    }

    public void PaintShieldAt(Vector3 worldPos, Color color)
    {
        paintDecalShield.Color = color;
        paintDecalShield.HandleHitPoint(false, 0, 1f, Random.Range(int.MinValue, int.MaxValue), worldPos, Quaternion.identity);
    }
}