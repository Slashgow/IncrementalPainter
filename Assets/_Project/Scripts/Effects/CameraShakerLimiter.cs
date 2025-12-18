using FirstGearGames.SmoothCameraShaker;
using inkolorgames;
using inkolorgames.effects;
using UnityEngine;
using UnityTimer;

public class CameraShakerLimiter : Effect
{
    public static bool isDoingEffect = false;

    [SerializeField] private ShakeData shakeData;

    private Timer shakerTimer;

    public override void DoEffect()
    {
        if(isDoingEffect)
            return;

        if (!GameSettingsManager.Instance.IsScreenShakeEnable)
            return;

        shakerTimer?.Cancel();
        CameraShakerHandler.Shake(shakeData);

        isDoingEffect = true;
        shakerTimer = Timer.Register(shakeData.TotalDuration, onComplete:() => { isDoingEffect = false; });
    }

    private void OnDestroy()
    {
        shakerTimer?.Cancel();
    }
}
