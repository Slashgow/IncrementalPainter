using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class UICurrentDay : MonoBehaviour
{
    [SerializeField] private LocalizedString dayLocalized;
    [SerializeField] private TextMeshProUGUI dayElapsedText;
    private void Start() => LevelCompletionTracker.OnUpdateDay += LevelCompletionTracker_OnUpdateDay;
    private void OnDestroy() => LevelCompletionTracker.OnUpdateDay -= LevelCompletionTracker_OnUpdateDay;
    private void LevelCompletionTracker_OnUpdateDay(int dayElapsed) => dayElapsedText.text = $"{dayLocalized.GetLocalizedString()} {dayElapsed}";
}
