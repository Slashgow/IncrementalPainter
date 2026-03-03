using UnityEngine;

public class Mine : MonoBehaviour
{
    private MineDamageor damageor;
    private bool hasExploded = false;
    public void Initialize(MineDamageor damageor)
    {
        this.damageor = damageor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(hasExploded) 
            return;

        if (damageor == null) 
            return;

        if ((damageor.DamageableLayers.value & (1 << other.gameObject.layer)) == 0) 
            return;

        if (other.GetComponentInParent<IDamageable>() == null) 
            return;

        damageor.NotifyMineExplosion(transform.position);
        damageor.TryDamage(transform.position);
        Debug.Log($"Mine exploded at {transform.position} dealing {damageor.Damage} damage with radius {damageor.DamageRadius}", this);
        hasExploded = true;
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (damageor == null) return;
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.35f);
        Gizmos.DrawSphere(transform.position, damageor.DamageRadius);
    }
#endif
}