using inkolorgames.effects;
using UnityEngine;
using UnityTimer;

public class SFXPitchShiftingEffect : Effect
{
    [Header("Currency Count Pitch Settings")]
    [SerializeField, Range(-3f, 3f)] private float minPitch = 1f;
    [SerializeField, Range(-3f, 3f)] private float maxPitch = 2f;
    [SerializeField, Range(0f, 3f)] private float pitchIncrement = 0.1f;
    [SerializeField, Range(0f, 3f)] private float pitchResetDelay = 0.3f;
    [SerializeField, Range(0f, 0.5f)] private float delayBetweenCalls = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;

    private float currentPitch = 1f;
    private Timer pitchResetTimer;
    private float previousTime;


    public override void DoEffect() => PlaySFXWithPitchShift();

    private void PlaySFXWithPitchShift()
    {
        if (Mathf.Abs(Time.unscaledTime - previousTime) < delayBetweenCalls)
            return;

        previousTime = Time.unscaledTime;
        pitchResetTimer?.Cancel();

        currentPitch = Mathf.Min(currentPitch + pitchIncrement, maxPitch);
        SFXManager.Instance.PlayAudioClipWithPitch(audioClip, currentPitch);

        pitchResetTimer = Timer.Register(pitchResetDelay, onComplete: () => currentPitch = minPitch, useRealTime: true);
    }
    private void OnDestroy() => pitchResetTimer?.Cancel();
}
