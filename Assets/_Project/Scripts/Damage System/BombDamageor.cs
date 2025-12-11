using System;
using UnityEngine;
using UnityEngine.Events;

public class BombDamageor : BaseDamageor
{
    public static event Action<float> OnBombDamage;

    public UnityEvent<Vector3> OnPaintBombDie;
    private void OnEnable() => PaintBlob.OnAnyPaintBombDie += PaintBlob_OnAnyPaintBombDie;
    private void OnDisable() => PaintBlob.OnAnyPaintBombDie -= PaintBlob_OnAnyPaintBombDie;
    private void PaintBlob_OnAnyPaintBombDie(Vector3 deathWorldPosition)
    {
        OnPaintBombDie?.Invoke(deathWorldPosition);
        TryDamage(deathWorldPosition);
    }

    public override void NotityDamage(float damage) => OnBombDamage?.Invoke(damage);
}