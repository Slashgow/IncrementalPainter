using UnityEngine;

public class RandomMovement : MonoBehaviour, ILevelContainable
{
    [Header("Movement Settings")]
    [SerializeField, Range(0f,20f)] private float moveSpeed = 2f;
    [SerializeField, Range(0f, 20f)] private float changeDirectionInterval = 2f;

    private Vector3 currentDirection;
    private float timeSinceLastDirectionChange;
    private Bounds spriteBounds;

    private void OnEnable()
    {
        spriteBounds = LevelManager.Instance.FrameBounds;
        PickNewDirection();
    }


    private void Update()
    {
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
}