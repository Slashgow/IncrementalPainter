using System;
using UnityEngine;

public class EaterEnemy : MonoBehaviour
{
    public static event Action<Vector3> OnEatAnyBlob;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PaintBlob blob))
        {
            IDamageable damageable = blob.GetComponentInParent<IDamageable>();
            if(damageable != null)
            {
                damageable.Die();
                OnEatAnyBlob?.Invoke(transform.position);
            }
        }
    }
}