using UnityEngine;

public class RotateCommand : ICommand
{
    private Transform targetTransform;
    private Quaternion previousRotation;
    private Quaternion newRotation;

    public RotateCommand(Transform transform, Quaternion fromRotation, Quaternion toRotation)
    {
        targetTransform = transform;
        previousRotation = fromRotation;
        newRotation = toRotation;
    }

    public void Execute()
    {
        if (targetTransform != null)
        {
            targetTransform.rotation = newRotation;
        }
    }

    public void Undo()
    {
        if (targetTransform != null)
        {
            targetTransform.rotation = previousRotation;
        }
    }
}

