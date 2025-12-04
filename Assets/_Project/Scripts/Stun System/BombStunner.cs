using UnityEngine;

public class BombStunner : BaseStunner
{
    private void OnEnable() => PaintBlob.OnAnyPaintFreezerDie += PaintBlob_OnAnyPaintFreezerDie;
    private void OnDisable() => PaintBlob.OnAnyPaintFreezerDie -= PaintBlob_OnAnyPaintFreezerDie;

    private void PaintBlob_OnAnyPaintFreezerDie(Vector3 deathWorldPosition) => TryStun(deathWorldPosition);
}
