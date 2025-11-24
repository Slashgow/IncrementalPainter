using UnityEngine;

public class LevelEdgeDetector : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collider2D)
    {
        if(collider2D.TryGetComponent(out ILevelContainable levelContainable))
        {
            Debug.Log("Reversing direction for " + collider2D.name);
            levelContainable.ReverseDirection();
        }
    }
}
