using UnityEngine;

public class ScaleCommand : ICommand
{
    private Transform targetTransform;
    private Vector3 previousScale;
    private Vector3 newScale;

    public ScaleCommand(Transform transform, Vector3 fromScale, Vector3 toScale)
    {
        targetTransform = transform;
        previousScale = fromScale;
        newScale = toScale;
    }

    public void Execute()
    {
        if (targetTransform != null)
        {
            targetTransform.localScale = newScale;
        }
    }

    public void Undo()
    {
        if (targetTransform != null)
        {
            targetTransform.localScale = previousScale;
        }
    }
}

