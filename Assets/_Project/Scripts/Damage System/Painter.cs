using inkolorgames;
using PaintIn2D;
using UnityEngine;

[DisallowMultipleComponent]
public class Painter : MonoSingleton<Painter>
{
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> splatterScalePerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SplatterScalePerLevel => splatterScalePerLevel;
    public Vector3 SplatterScale => Vector3.one * splatterScalePerLevel.GetCurrentLevelData();


    [SerializeField] private CwPaintDecal2D paintDecal;

    private void OnEnable() => SimpleDamageable.OnAnyDamageableDie += HandleOnDie;
    private void OnDisable() => SimpleDamageable.OnAnyDamageableDie -= HandleOnDie;

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
}