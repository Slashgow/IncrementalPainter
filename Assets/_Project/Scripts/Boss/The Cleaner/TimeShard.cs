using UnityEngine;

public class TimeShard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SimpleDamageable damageable;

    [Header("Configuration")]
    [SerializeField] private float timeAmount = 5f;

    private void OnEnable() => damageable.OnDie += HandleDie;
    private void OnDisable() => damageable.OnDie -= HandleDie;
    public void SetTimeAmount(float amount) => timeAmount = amount;

    private void HandleDie(Vector3 position, Color color)
    {
        PaintStateManager.Instance.AddTimeToCountdown(timeAmount);
        Debug.Log($"Time shard destroyed - added {timeAmount} seconds to countdown!");
    }
}