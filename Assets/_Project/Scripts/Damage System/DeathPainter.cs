using PaintIn2D;
using UnityEngine;

[DisallowMultipleComponent]
public class DeathPainter : MonoBehaviour
{
    [SerializeField] private CwPaintDecal2D paintDecal;

    private void OnEnable() => SimpleDamageable.OnAnyDamageableDie += HandleOnDie;
    private void OnDisable() => SimpleDamageable.OnAnyDamageableDie -= HandleOnDie;
    private void HandleOnDie(Vector3 worldPos, Color color) => PaintAt(worldPos, color);

    private void PaintAt(Vector3 worldPos, Color color)
    {
        paintDecal.Color = color;
        paintDecal.HandleHitPoint(false, 0, 1f, Random.Range(int.MinValue, int.MaxValue), worldPos, Quaternion.identity);
    }
}