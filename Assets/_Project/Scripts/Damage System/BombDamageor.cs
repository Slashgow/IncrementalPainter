using System;
using UnityEngine;
using UnityEngine.Events;

public class BombDamageor : BaseDamageor
{
    public static event Action<float> OnBombDamage;

    public UnityEvent<Vector3, Color> OnPaintBombDie;
    private void OnEnable() => PaintBlob.OnAnyPaintBombDie += PaintBlob_OnAnyPaintBombDie;
    private void OnDisable() => PaintBlob.OnAnyPaintBombDie -= PaintBlob_OnAnyPaintBombDie;
    private void PaintBlob_OnAnyPaintBombDie(Vector3 deathWorldPosition, Color color)
    {
        OnPaintBombDie?.Invoke(deathWorldPosition, color);
        TryDamage(deathWorldPosition);
    }

    public override void NotityDamage(float damage) => OnBombDamage?.Invoke(damage);
}
