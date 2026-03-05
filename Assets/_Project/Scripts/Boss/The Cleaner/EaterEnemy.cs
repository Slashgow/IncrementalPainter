using System;
using UnityEngine;

public class EaterEnemy : MonoBehaviour
{
    [SerializeField, Range(0,2f)] private float minTimeBetweenEat = 0.2f;
    public static event Action<Vector3> OnEatAnyBlob;

    private float timeElapsedSinceLastEat = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeElapsedSinceLastEat < minTimeBetweenEat)
            return;

        if(collision.TryGetComponent(out PaintBlob blob))
        {
            IDamageable damageable = blob.GetComponentInParent<IDamageable>();
            if(damageable != null)
            {
                damageable.Die();
                OnEatAnyBlob?.Invoke(transform.position);
                timeElapsedSinceLastEat = 0f;
            }
        }
    }

    private void Update()
    {
        timeElapsedSinceLastEat += Time.deltaTime;
    }
}