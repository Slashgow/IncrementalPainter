using System;
using UnityEngine;

[Serializable]
public class PoisonZoneData
{
    private float radius;
    private float damagePerTick;
    private float duration;
    private float tickInterval;
    private float criticalHitLuck;
    private float criticalDamageMultiplier;
    private Color zoneColor;
    private LayerMask damageableLayers;

    public PoisonZoneData(float radius, float damagePerTick, float duration, float tickInterval, float criticalHitLuck, 
        float criticalDamageMultiplier, Color zoneColor, LayerMask damageableLayers)
    {
        this.radius = radius;
        this.damagePerTick = damagePerTick;
        this.duration = duration;
        this.tickInterval = tickInterval;
        this.criticalHitLuck = criticalHitLuck;
        this.criticalDamageMultiplier = criticalDamageMultiplier;
        this.zoneColor = zoneColor;
        this.damageableLayers = damageableLayers;
    }

    public float Radius => radius;
    public float DamagePerTick => damagePerTick;
    public float Duration => duration;
    public float TickInterval => tickInterval;
    public float CriticalHitLuck => criticalHitLuck;
    public float CriticalDamageMultiplier => criticalDamageMultiplier;
    public Color ZoneColor => zoneColor;
    public LayerMask DamageableLayers => damageableLayers;
}
