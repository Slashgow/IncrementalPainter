using inkolorgames;
using UnityEngine;

public class AdvancedGameSettingsManager : MonoSingleton<AdvancedGameSettingsManager>
{
    [SerializeField, Range(0f,1f)] private float defaultFontSizeMultiplier = 1.0f;
    [SerializeField] private bool defaultScreenShake = true;

    public bool IsScreenShakeEnable { get; private set; }

    public const string IS_SCREEN_SHAKE_ENABLE_ID = "IsScreenShakeEnable";

    public float FontSizeMultiplier { get; private set; }
    public const string FONT_SIZE_MULTIPLIER_ID = "FontSizeMultiplier";

    protected override void Awake()
    {
        base.Awake();

        IsScreenShakeEnable = PlayerPrefs.GetInt(IS_SCREEN_SHAKE_ENABLE_ID, defaultScreenShake ? 1 : 0) == 1;
        FontSizeMultiplier = PlayerPrefs.GetFloat(FONT_SIZE_MULTIPLIER_ID, defaultFontSizeMultiplier);
        //Debug.Log($"FontSizeMultiplier on awake : {FontSizeMultiplier}");
    }

    private void Start()
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        SetIsScreenShakeEnable(IsScreenShakeEnable);
        SetFontSizeMultiplier(FontSizeMultiplier);
    }

    public void SetIsScreenShakeEnable(bool value)
    {
        IsScreenShakeEnable = value;
        PlayerPrefs.SetInt(IS_SCREEN_SHAKE_ENABLE_ID, IsScreenShakeEnable ? 1 : 0);
    }

    public void SetFontSizeMultiplier(float value)
    {
        FontSizeMultiplier = value;
        PlayerPrefs.SetFloat(FONT_SIZE_MULTIPLIER_ID, FontSizeMultiplier);
    }
}
