using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    private Vector3 mousePosition;
    public float moveSpeed = 0.1f;

    private void Update()
    {
        if (GameManager.CurrentPauseState == GameManager.PauseState.PAUSE)
            return;

        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
    }
}
