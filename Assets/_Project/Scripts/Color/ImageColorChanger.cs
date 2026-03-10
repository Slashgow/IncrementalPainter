using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorChanger : MonoBehaviour, IColorChanger
{
    [SerializeField] private ColorId colorId = ColorId.PRIMARY;
    [SerializeField] private bool updateOnStart = true;
    [SerializeField] private bool updateOnEnable = false;
    [SerializeField] private bool listenToThemeChanges = true;

    private Image targetImage;

    public ColorId ColorId => colorId;

    private void Awake() => targetImage = GetComponent<Image>();

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

        if(updateOnEnable)
            UpdateColor();
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
        if (targetImage != null)
        {
            targetImage.color = color;
        }
    }


    // Editor button support
#if UNITY_EDITOR
    [ContextMenu("Update Color Now")]
    private void UpdateColorEditor()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }
        UpdateColor();
    }
#endif
}
