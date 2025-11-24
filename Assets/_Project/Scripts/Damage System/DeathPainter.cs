using PaintIn2D;
using UnityEngine;

[DisallowMultipleComponent]
public class DeathPainter : MonoBehaviour
{
    [SerializeField] private CwPaintDecal2D paintDecal;

    private void OnEnable() => SimpleDamageable.OnAnyDamageableDie += HandleOnDie;
    private void OnDisable() => SimpleDamageable.OnAnyDamageableDie -= HandleOnDie;
    private void HandleOnDie(Vector3 worldPos) => PaintAt(worldPos);

    private void PaintAt(Vector3 worldPos)
    {
        paintDecal.HandleHitPoint(false, 0, 1f, Random.Range(int.MinValue, int.MaxValue), worldPos, Quaternion.identity);
    }
}