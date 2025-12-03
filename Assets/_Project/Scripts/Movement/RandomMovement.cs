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

        newPosition.x = Mathf.Clamp(newPosition.x, spriteBounds.min.x, spriteBounds.max.x);
        newPosition.y = Mathf.Clamp(newPosition.y, spriteBounds.min.y, spriteBounds.max.y);

        transform.position = newPosition;
    }

    private void PickNewDirection()
    {
        currentDirection = Random.insideUnitCircle.normalized;
    }

    public void ReverseDirection() => currentDirection = -currentDirection;
    public void StartMovement() => canMove = true;
    public void StopMovement() => canMove = false;
}