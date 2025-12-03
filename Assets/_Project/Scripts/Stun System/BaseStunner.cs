using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseStunner : MonoBehaviour, IStunner
{
    [Header("Stun Parameters")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> stunDurationSkillDataPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> stunRadiusSkillDataPerLevel;

    public SkillDataPerLevelOfType<FunctionAffine> StunDurationSkillDataPerLevel => stunDurationSkillDataPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> StunRadiusSkillDataPerLevel => stunRadiusSkillDataPerLevel;

    [SerializeField] private LayerMask stunnableLayers = ~0;

    public float StunDuration => stunDurationSkillDataPerLevel.GetCurrentLevelData();
    public float StunRadius => stunRadiusSkillDataPerLevel.GetCurrentLevelData();

    public UnityEvent OnStunAppliedOnce;
    public static event Action<float, Vector3> OnAnyStunnerApplied;

    public void TryStun(Vector3 position)
    {
        TryStunFromWorldPoint(position);
    }

    private void TryStunFromWorldPoint(Vector3 worldPos)
    {
        Vector2 worldPosition2D = new Vector2(worldPos.x, worldPos.y);
        Collider2D[] colliders2D = Physics2D.OverlapCircleAll(worldPosition2D, StunRadius, stunnableLayers.value);

        bool anyStunApplied = false;

        foreach (var collider in colliders2D)
        {
            var stunnable = collider.GetComponentInParent<IStunnable>();
            if (stunnable != null)
            {
                anyStunApplied = true;
                stunnable.ApplyStun(StunDuration);
                OnAnyStunnerApplied?.Invoke(StunDuration, collider.transform.position);
            }
        }

        if (anyStunApplied)
            OnStunAppliedOnce?.Invoke();
    }
}
