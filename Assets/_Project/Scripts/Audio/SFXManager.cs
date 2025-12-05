using System.Collections.Generic;
using inkolorgames;
using inkolorgames.effects;
using UnityEngine;

public class SFXManager : BaseSFXManager
{
    [SerializeField] private AudioClip hoverAudioClip, clickAudioClick;

    protected override void Awake()
    {
        base.Awake();

        SelectableInputRegister.OnAnySelectableHover += SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed += SelectableInputRegister_OnAnySelectablePressed;
    }

    private void OnDestroy()
    {
        SelectableInputRegister.OnAnySelectableHover -= SelectableInputRegister_OnAnySelectableHover;
        SelectableInputRegister.OnAnySelectablePressed -= SelectableInputRegister_OnAnySelectablePressed;
    }

    private void SelectableInputRegister_OnAnySelectablePressed() => PlayAudioClip(clickAudioClick);
    private void SelectableInputRegister_OnAnySelectableHover() => PlayAudioClip(hoverAudioClip);

    public void PlayRandomAudioClipWithPitch(List<AudioClip> audioClips, float pitch)
    {
        int randomIndex = Random.Range(0, audioClips.Count);
        AudioClip randomClip = audioClips[randomIndex];
        PlayAudioClipWithPitch(randomClip, pitch);
    }
}
