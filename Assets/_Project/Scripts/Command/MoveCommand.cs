using UnityEngine;

public class MoveCommand : ICommand
{
    private Transform targetTransform;
    private Vector3 previousPosition;
    private Vector3 newPosition;

    public MoveCommand(Transform transform, Vector3 fromPosition, Vector3 toPosition)
    {
        targetTransform = transform;
        previousPosition = fromPosition;
        newPosition = toPosition;
    }

    public void Execute()
    {
        if (targetTransform != null)
        {
            targetTransform.position = newPosition;
        }
    }

    public void Undo()
    {
        if (targetTransform != null)
        {
            targetTransform.position = previousPosition;
        }
    }
}
