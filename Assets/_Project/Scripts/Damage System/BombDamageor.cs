using UnityEngine;

public class BombDamageor : BaseDamageor
{
    private void OnEnable() => PaintBlob.OnAnyPaintBombDie += PaintBlob_OnAnyPaintBombDie;
    private void OnDisable() => PaintBlob.OnAnyPaintBombDie -= PaintBlob_OnAnyPaintBombDie;
    private void PaintBlob_OnAnyPaintBombDie(Vector3 deathWorldPosition) => TryDamage(deathWorldPosition);
}