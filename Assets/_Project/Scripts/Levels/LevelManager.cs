using inkolorgames;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
{
    [SerializeField] private SpriteRenderer frameRenderer;

    public SpriteRenderer FrameRenderer => frameRenderer;
    public Bounds FrameBounds => frameRenderer.bounds;

}
