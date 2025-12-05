using inkolorgames.effects;
using UnityEngine;

public class SFXPitchEffect : Effect
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField, Range(0f,2f)] private float minPitch, maxPitch;

    public override void DoEffect()
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip randomClip = audioClips[randomIndex];
        SFXManager.Instance.PlayAudioClipWithPitch(randomClip, Random.Range(minPitch, maxPitch));
    }
}
