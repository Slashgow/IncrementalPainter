using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererColorChanger : MonoBehaviour, IColorChanger
{
    [SerializeField] private ColorId colorId = ColorId.PRIMARY;
    [SerializeField] private bool updateOnStart = true;
    [SerializeField] private bool listenToThemeChanges = true;

    private SpriteRenderer targetRenderer;

    public ColorId ColorId => colorId;

    private void Awake()
    {
        targetRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (updateOnStart)
        {
            UpdateColor();
        }
    }

    private void OnEnable()
    {
        if (listenToThemeChanges)
        {
            ThemeColorManager.OnThemeChanged += OnThemeChanged;
            ThemeColorManager.OnColorChanged += OnSpecificColorChanged;
        }
    }

    private void OnDisable()
    {
        if (listenToThemeChanges)
        {
            ThemeColorManager.OnThemeChanged -= OnThemeChanged;
            ThemeColorManager.OnColorChanged -= OnSpecificColorChanged;
        }
    }

    public void OnThemeChanged(ColorTheme newTheme)
    {
        UpdateColor();
    }

    public void OnSpecificColorChanged(ColorId id, Color color)
    {
        if (id == colorId)
        {
            SetColor(color);
        }
    }

    public void UpdateColor()
    {
        if (ThemeColorManager.Instance == null)
        {
            Debug.LogWarning("ThemeColorManager instance not found!");
            return;
        }

        Color color = ThemeColorManager.Instance.GetColor(colorId);
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        if (targetRenderer != null)
        {
            targetRenderer.color = color;
        }
    }
#if UNITY_EDITOR
    [ContextMenu("Update Color Now")]
    private void UpdateColorEditor()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }
        UpdateColor();
    }
#endif
}
