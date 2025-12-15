using UnityEngine;
using UnityEngine.Events;

public class BombStunner : BaseStunner
{
    public UnityEvent<Vector3> OnPaintFreezerDie;
    private void OnEnable() => PaintBlob.OnAnyPaintFreezerDie += PaintBlob_OnAnyPaintFreezerDie;
    private void OnDisable() => PaintBlob.OnAnyPaintFreezerDie -= PaintBlob_OnAnyPaintFreezerDie;

    private void PaintBlob_OnAnyPaintFreezerDie(Vector3 deathWorldPosition)
    {
        OnPaintFreezerDie?.Invoke(deathWorldPosition);
        TryStun(deathWorldPosition);
    }
}
