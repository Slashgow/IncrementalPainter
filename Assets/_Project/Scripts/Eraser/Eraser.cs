using inkolorgames;
using PaintIn2D;
using UnityEngine;

public class Eraser : MonoSingleton<Eraser>
{
    [SerializeField] private CwPaintDecal2D paintDecalEraser;

    public void EraseAt(Vector3 worldPos, float eraserScale)
    {
        paintDecalEraser.Scale = Vector3.one * eraserScale;
        paintDecalEraser.HandleHitPoint(false, 0, 1f, Random.Range(int.MinValue, int.MaxValue), worldPos, Quaternion.identity);
    }
}