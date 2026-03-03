using UnityEngine;

public class RandomMovement : MonoBehaviour, ILevelContainable, IMovable
{
    [Header("Movement Settings")]
    [SerializeField, Range(0f,20f)] private float moveSpeed = 2f;
    [SerializeField, Range(0f, 20f)] private float changeDirectionInterval = 2f;

    private Vector3 currentDirection;
    private float timeSinceLastDirectionChange;
    private Bounds spriteBounds;
    private bool canMove;

    private const float MIN_CLAMP_OFFSET = 0.1f;
    private void OnEnable()
    {
        StartMovement();
        PickNewDirection();
    }

    private void Update()
    {
        if (!canMove)
            return;

        timeSinceLastDirectionChange += Time.deltaTime;

        if (timeSinceLastDirectionChange >= changeDirectionInterval)
        {
            PickNewDirection();
            timeSinceLastDirectionChange = 0f;
        }

        MoveObject();
    }

    private void MoveObject()
    {
        spriteBounds = LevelManager.Instance.FrameBounds;

        Vector3 newPosition = transform.position + currentDirection * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, spriteBounds.min.x - MIN_CLAMP_OFFSET, spriteBounds.max.x + MIN_CLAMP_OFFSET);
        newPosition.y = Mathf.Clamp(newPosition.y, spriteBounds.min.y - MIN_CLAMP_OFFSET, spriteBounds.max.y + MIN_CLAMP_OFFSET);

        transform.position = newPosition;
    }

    private void PickNewDirection()
    {
        currentDirection = Random.insideUnitCircle.normalized;
    }

    public void ReverseDirection()
    {
        currentDirection = (Vector3.zero - transform.position).normalized;
        timeSinceLastDirectionChange = 0f;
    }

    public void ReflectDirection(Vector3 hitNormal)
    {
        //currentDirection = Vector3.Reflect(currentDirection, hitNormal).normalized;
        float cosTheta = Vector3.Dot(-currentDirection, hitNormal); // cos of angle of incidence
        currentDirection = (currentDirection + 2f * cosTheta * hitNormal).normalized;
        //Debug.Log($"Reflected direction: {currentDirection}");
        timeSinceLastDirectionChange = 0f;
    }

    public void StartMovement() => canMove = true;
    public void StopMovement() => canMove = false;

    private void OnDrawGizmos()
    {
        if (currentDirection == Vector3.zero) return;

        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position;
        Vector3 end = origin + currentDirection * 1.5f;

        Gizmos.DrawLine(origin, end);
        Gizmos.DrawSphere(end, 0.1f);
    }
}