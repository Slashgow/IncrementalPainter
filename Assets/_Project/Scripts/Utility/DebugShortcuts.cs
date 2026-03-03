using inkolorgames;
using UnityEngine;

public class DebugShortcuts : PersistentMonoSingleton<DebugShortcuts>
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD

    [Header("Time Scale Presets")]
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private float fastSpeed = 3f;
    [SerializeField] private float veryFastSpeed = 5f;
    [SerializeField] private float slowSpeed = 0.25f;

    private float previousTimeScale = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetTimeScale(normalSpeed, "Normal");
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetTimeScale(fastSpeed, "Fast");
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetTimeScale(veryFastSpeed, "Very Fast");
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetTimeScale(slowSpeed, "Slow Motion");
        if (Input.GetKeyDown(KeyCode.Alpha5)) TogglePause();
    }

    private void SetTimeScale(float scale, string label)
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;
        Debug.Log($"[DebugShortcuts] Time scale ? {label} (x{scale})");
    }

    private void TogglePause()
    {
        if (Time.timeScale != 0f)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Debug.Log("[DebugShortcuts] Paused");
        }
        else
        {
            Time.timeScale = previousTimeScale;
            Time.fixedDeltaTime = 0.02f * previousTimeScale;
            Debug.Log($"[DebugShortcuts] Resumed (x{previousTimeScale})");
        }
    }

    private void OnDestroy()
    {
        // Safety: always restore time scale when this object is destroyed
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

#endif
}