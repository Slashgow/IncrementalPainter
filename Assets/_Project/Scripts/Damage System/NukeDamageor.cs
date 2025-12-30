using System;
using UnityEngine;
using UnityEngine.Events;

public class NukeDamageor : BaseDamageor
{
    [SerializeField] private UnityEvent<Vector3> OnCollectNuke;

    public static event Action<float> OnNukeDamage;
    public override void NotityDamage(float damage) => OnNukeDamage?.Invoke(damage);

    private void OnEnable() => NukeItem.OnNukeCollectedEvent += NukeItem_OnNukeCollectedEvent;
    private void OnDisable() => NukeItem.OnNukeCollectedEvent -= NukeItem_OnNukeCollectedEvent;
    private void NukeItem_OnNukeCollectedEvent(Vector3 collectWorldPosition)
    {
        OnCollectNuke?.Invoke(collectWorldPosition);

        TryDamage(collectWorldPosition);
    }
}
