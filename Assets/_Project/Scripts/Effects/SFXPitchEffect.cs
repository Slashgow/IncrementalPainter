using inkolorgames.effects;
using UnityEngine;

public class SFXPitchEffect : Effect
{
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField, Range(0f,2f)] private float minPitch, maxPitch;
    [SerializeField, Range(0f,1f)] private float volume = 1f;

    SFXManager sfxManager;

    private void Start()
    {
        sfxManager = (SFXManager)SFXManager.Instance;
    }

    public override void DoEffect()
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip randomClip = audioClips[randomIndex];
        sfxManager.PlayAudioClipWithPitch(randomClip, Random.Range(minPitch, maxPitch), volume);
    }
}
