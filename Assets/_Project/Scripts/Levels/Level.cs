using PaintCore;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Collider2D frameCollider;
    [SerializeField] private SpriteRenderer frameRenderer;
    [SerializeField] private SpriteRenderer drawingRenderer;
    [SerializeField] private CwChangeCounter colorChangeCounter;

    private LevelData levelData;

    public SpriteRenderer FrameRenderer => frameRenderer;
    public Collider2D FrameCollider => frameCollider;
    public CwChangeCounter ColorChangeCounter => colorChangeCounter;

    public void Initialize(LevelData levelData)
    {
        this.levelData = levelData;

        drawingRenderer.sprite = levelData.LevelDrawing;
    }
}
