using UnityEngine;

[CreateAssetMenu(fileName = "Gizmo Positions", menuName = "InkolorGames/Gizmo Position")]
public class GizmosPositions : ScriptableObject
{
   [Header("Manual Gizmo Positions (Local Space)")]
   [SerializeField] private Vector3 bottomLeftGizmoPosition = new Vector3(-1, -1, 0);
   [SerializeField] private Vector3 bottomRightGizmoPosition = new Vector3(1, -1, 0);
   [SerializeField] private Vector3 topRightGizmoPosition = new Vector3(1, 1, 0);
   [SerializeField] private Vector3 topLeftGizmoPosition = new Vector3(-1, 1, 0);
   [SerializeField] private Vector3 rotateGizmoPosition = new Vector3(0, 2, 0);

    public Vector3 BottomLeftGizmoPosition => bottomLeftGizmoPosition;
    public Vector3 BottomRightGizmoPosition => bottomRightGizmoPosition;
    public Vector3 TopRightGizmoPosition => topRightGizmoPosition;
    public Vector3 TopLeftGizmoPosition => topLeftGizmoPosition;
    public Vector3 RotateGizmoPosition => rotateGizmoPosition;

    private Vector3[] manualPositions;

    public Vector3[] ManualPositions
    {
        get 
        { 
            if(manualPositions == null || manualPositions.Length == 0)
            {
               manualPositions = new Vector3[] 
               {    bottomLeftGizmoPosition, 
                   bottomRightGizmoPosition, 
                   topRightGizmoPosition, 
                   topLeftGizmoPosition 
               };
            }
            return manualPositions; 
        }
    }
}
