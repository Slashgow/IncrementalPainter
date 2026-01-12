using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBackgroundColorChanger : MonoBehaviour, IColorChanger
{
    [SerializeField] private ColorId colorId = ColorId.UI_SECONDARY;
    [SerializeField] private bool updateOnStart = true;
    [SerializeField] private bool listenToThemeChanges = true;
    [SerializeField] private bool setSolidColor = true; 

    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();

        if (setSolidColor)
        {
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
        }
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
        }
    }

    private void OnDisable()
    {
        if (listenToThemeChanges)
        {
            ThemeColorManager.OnThemeChanged -= OnThemeChanged;
        }
    }

    public void OnThemeChanged(ColorTheme newTheme)
    {
        UpdateColor();
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

    private void SetColor(Color color)
    {
        if (targetCamera != null)
        {
            targetCamera.backgroundColor = color;
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Update Color Now")]
    private void UpdateColorEditor()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }

        if (setSolidColor)
        {
            targetCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        UpdateColor();
    }


#endif
}