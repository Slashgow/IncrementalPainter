using PaintCore;
using UnityEngine;
using UnityEngine.UI;

public class UIColorChangeProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBarImage;

    private CwChangeCounter counter;

    private void Start()
    {
        counter = LevelManager.Instance.CurrentLevel.ColorChangeCounter;
        counter.OnUpdated += ColorChangeCounter_OnUpdated;
    }
    private void OnDestroy()
    {
        if(counter != null)
            counter.OnUpdated -= ColorChangeCounter_OnUpdated;
    }

    private void ColorChangeCounter_OnUpdated() => progressBarImage.fillAmount = counter.Ratio;
}
