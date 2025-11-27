using PaintCore;
using UnityEngine;
using UnityEngine.UI;

public class UIColorChangeProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBarImage;
    [SerializeField] private CwChangeCounter colorChangeCounter;
    private void Start()
    {
        colorChangeCounter.OnUpdated += ColorChangeCounter_OnUpdated;
    }
    private void OnDestroy()
    {
        colorChangeCounter.OnUpdated -= ColorChangeCounter_OnUpdated;
    }

    private void ColorChangeCounter_OnUpdated() => progressBarImage.fillAmount = colorChangeCounter.Ratio;
}
