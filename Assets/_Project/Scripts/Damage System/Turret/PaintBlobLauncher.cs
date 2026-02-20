using UnityEngine;

[RequireComponent(typeof(RandomMovement))]
public class PaintBlobLauncher : MonoBehaviour
{
    [SerializeField] private RandomMovement randomMovement;
    [SerializeField, Range(1f, 40f)] private float launchSpeed = 10f;
    [SerializeField, Range(0f, 1f)] private float arrivalThreshold = 0.1f;

    private Vector3 targetPosition;
    public void Launch(Vector3 target)
    {
        targetPosition = target;
        randomMovement.StopMovement();
        enabled = true;
    }

    private void Update()
    {
        Vector3 toTarget = targetPosition - transform.position;

        if (toTarget.magnitude <= arrivalThreshold)
        {
            Arrive();
            return;
        }

        transform.position += toTarget.normalized * launchSpeed * Time.deltaTime;
    }

    private void Arrive()
    {
        transform.position = targetPosition;
        randomMovement.StartMovement();
        enabled = false;
    }

    private void OnDisable()
    {
        // Safety: make sure RandomMovement is running if this gets disabled externally (e.g. pooled away)
        randomMovement?.StartMovement();
    }
}
