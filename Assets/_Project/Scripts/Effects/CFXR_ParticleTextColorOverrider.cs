using CartoonFX;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(CFXR_ParticleText))]
public class CFXR_ParticleTextColorOverrider : MonoBehaviour
{
    [SerializeField] private CFXR_ParticleText particleText;

    [SerializeField] private bool overrideBackgroundColor = true;
    [SerializeField, ShowIf("overrideBackgroundColor")] private ColorId backgroundColorId;
    [SerializeField] private bool overrideColor1 = true;
    [SerializeField, ShowIf("overrideColor1")] private ColorId color1Id;
    [SerializeField] private bool overrideColor2 = true;
    [SerializeField, ShowIf("overrideColor2")] private ColorId color2Id;

    private void OnEnable()
    {
        if(overrideBackgroundColor)
            particleText.backgroundColor = ThemeColorManager.Instance.GetColor(backgroundColorId);
        if(overrideColor1)
            particleText.color1 = ThemeColorManager.Instance.GetColor(color1Id);
        if(overrideColor2)
            particleText.color2 = ThemeColorManager.Instance.GetColor(color2Id);

        particleText.UpdateText();
    }
}
