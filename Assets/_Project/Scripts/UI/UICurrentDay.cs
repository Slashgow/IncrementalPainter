using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class UICurrentDay : MonoBehaviour
{
    [SerializeField] private LocalizedString dayLocalized;
    [SerializeField] private TextMeshProUGUI dayElapsedText;
    [SerializeField] private bool showCurrentDay = false;

    private void Awake() => LevelCompletionTracker.OnUpdateDay += LevelCompletionTracker_OnUpdateDay;

    private void Start()
    {
        LevelCompletionTracker_OnUpdateDay(0);
    }
    private void OnDestroy() => LevelCompletionTracker.OnUpdateDay -= LevelCompletionTracker_OnUpdateDay;
    private void LevelCompletionTracker_OnUpdateDay(int dayElapsed)
    {
        dayElapsedText.text = $"{dayLocalized.GetLocalizedString()} {(showCurrentDay ? dayElapsed + 1 : dayElapsed)}";
    }
}
