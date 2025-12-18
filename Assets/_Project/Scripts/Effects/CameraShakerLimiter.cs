using FirstGearGames.SmoothCameraShaker;
using inkolorgames;
using inkolorgames.effects;
using UnityEngine;
using UnityTimer;

public class CameraShakerLimiter : Effect
{
    private static bool isDoingEffect = false;
    private static Timer shakerTimer;
    private static int activeInstanceCount = 0;

    [SerializeField] private ShakeData shakeData;

    private void Awake()
    {
        activeInstanceCount++;
    }

    public override void DoEffect()
    {
        if (isDoingEffect)
            return;

        if (!GameSettingsManager.Instance.IsScreenShakeEnable)
            return;

        shakerTimer?.Cancel();
        CameraShakerHandler.Shake(shakeData);

        isDoingEffect = true;
        shakerTimer = Timer.Register(shakeData.TotalDuration, onComplete: () =>
        {
            isDoingEffect = false;
            shakerTimer = null;
        });
    }

    private void OnDestroy()
    {
        activeInstanceCount--;

        // Only clean up if this was the last instance
        if (activeInstanceCount <= 0)
        {
            shakerTimer?.Cancel();
            isDoingEffect = false;
            shakerTimer = null;
            activeInstanceCount = 0; // Safety reset
        }
    }

    // Optional: Static method to force reset
    public static void ForceReset()
    {
        shakerTimer?.Cancel();
        isDoingEffect = false;
        shakerTimer = null;
    }

    // Optional: Check if currently shaking
    public static bool IsShaking => isDoingEffect;
}
