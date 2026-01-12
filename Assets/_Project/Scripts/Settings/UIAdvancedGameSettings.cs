using inkolorgames;
using UnityEngine;
using UnityEngine.UI;

public class UIAdvancedGameSettings : MonoSingleton<UIAdvancedGameSettings>
{
    [SerializeField] private Toggle enableScreenShakeToggle;
    [SerializeField] private Slider fontSizeMultiplierSlider;

   protected virtual void LoadSettings()
   {
        enableScreenShakeToggle.isOn = AdvancedGameSettingsManager.Instance.IsScreenShakeEnable;
        fontSizeMultiplierSlider.value = AdvancedGameSettingsManager.Instance.FontSizeMultiplier;
   }
   
   public void SetFontSizeMultiplier(float value) => AdvancedGameSettingsManager.Instance.SetFontSizeMultiplier(value);
    public void SetScreenShake(bool value) => AdvancedGameSettingsManager.Instance.SetIsScreenShakeEnable(value);
}
