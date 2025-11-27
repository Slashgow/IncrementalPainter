using inkolorgames.effects;
using UnityEngine;

public class SFXRandomEffect : SFXEffects
{
    [SerializeField, Range(0f,100f)] private float luckPercentage = 35f;
    public override void DoEffect()
    {
        if(LuckUtility.RollLuck(luckPercentage))
            base.DoEffect();
    }
}
