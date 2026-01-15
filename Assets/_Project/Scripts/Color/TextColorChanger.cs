using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextColorChanger : MonoBehaviour, IColorChanger
{
    [SerializeField] private ColorId colorId = ColorId.TEXT;
    [SerializeField] private bool updateOnStart = true;
    [SerializeField] private bool listenToThemeChanges = true;

    private TextMeshProUGUI targetText;

    public ColorId ColorId => colorId;

    private void Awake()
    {
        targetText = GetComponent<TextMeshProUGUI>();
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
        //UpdateColor();
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
        if (targetText != null)
        {
            targetText.color = color;
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Update Color Now")]
    private void UpdateColorEditor()
    {
        if (targetText == null)
        {
            targetText = GetComponent<TextMeshProUGUI>();
        }
        UpdateColor();
    }
#endif
}

