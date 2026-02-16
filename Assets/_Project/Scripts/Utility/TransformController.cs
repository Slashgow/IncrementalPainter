using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransformController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Gizmo Prefabs")]
    [SerializeField] private GameObject scaleGizmoPrefab;
    [SerializeField] private GameObject rotateGizmoPrefab;
    [SerializeField] private GameObject destroyGizmoPrefab;

    [Header("Gizmo Settings")]
    [SerializeField] private float gizmoSize = 0.3f;
    [SerializeField] private float rotateHandleDistance = 1.5f;

    [Header("Manual Positioning")]
    [SerializeField] private bool useManualGizmoPositions = false;
    [Tooltip("When enabled, gizmos will use the manually defined positions below instead of auto-calculating from bounds")]

    [Header("Manual Gizmo Positions (Local Space)")]
    [SerializeField] private GizmosPositions gizmosPositions;

    [Header("Scale Constraints")]
    [SerializeField] private float minScale = 0.3f;
    [SerializeField] private float maxScale = 3.0f;
    [SerializeField] private bool maintainAspectRatio = true;

    [Header("Rotation Settings")]
    [SerializeField] private bool snapRotation = false;
    [SerializeField] private float rotationSnapAngle = 15f;

    [Header("References")]
    [SerializeField] private Transform targetTransform;

    private List<TransformGizmo> scaleGizmos = new List<TransformGizmo>();
    private TransformGizmo destroyGizmo;
    private TransformGizmo rotateGizmo;
    private bool gizmosVisible = true;
    private Bounds targetBounds;

    public float MinScale => minScale;
    public float MaxScale => maxScale;
    public bool MaintainAspectRatio => maintainAspectRatio;
    public bool SnapRotation => snapRotation;
    public float RotationSnapAngle => rotationSnapAngle;

    private readonly Vector3[] CORNER_OFFSETS = new Vector3[]
    {
                new Vector3(-1, -1, 0),
                new Vector3(1, -1, 0),
                new Vector3(1, 1, 0),
                new Vector3(-1, 1, 0)
    };

    private void Start() => Initialize();

    public void Initialize()
    {
        CreateGizmos();
        UpdateGizmoPositions();
    }

    private void CreateGizmos()
    {
        if (targetTransform == null)
            return;

        ClearGizmos();

        if (!useManualGizmoPositions)
            CalculateTargetBounds();

        CreateDestroyGizmo();
        CreateCornerGizmos();
        CreateRotateGizmo();
    }

    private void CreateDestroyGizmo()
    {
        GameObject destroyGO = Instantiate(destroyGizmoPrefab, targetTransform);
        destroyGO.name = "DestroyGizmo";

        Vector3 localPos = useManualGizmoPositions ? gizmosPositions.DestroyGizmoPosition : new Vector3(0, -targetBounds.extents.y - rotateHandleDistance, 0);
        destroyGO.transform.localPosition = localPos;
        destroyGO.transform.localRotation = Quaternion.identity;

        destroyGizmo = destroyGO.GetComponent<TransformGizmo>();

        destroyGizmo.Initialize(targetTransform, this, TransformGizmo.GizmoType.Destroy);
    }

    private void CreateRotateGizmo()
    {
        GameObject rotateGO = Instantiate(rotateGizmoPrefab, targetTransform);
        rotateGO.name = "RotateGizmo";

        Vector3 localPos = useManualGizmoPositions ? gizmosPositions.RotateGizmoPosition : new Vector3(0, targetBounds.extents.y + rotateHandleDistance, 0);
        rotateGO.transform.localPosition = localPos;
        rotateGO.transform.localRotation = Quaternion.identity;

        rotateGizmo = rotateGO.GetComponent<TransformGizmo>();

        rotateGizmo.Initialize(targetTransform, this, TransformGizmo.GizmoType.RotateHandle);
    }

    private void CreateCornerGizmos()
    {
        Vector3[] cornerPositions;
        Vector3[] oppositeCorners;

        if (useManualGizmoPositions)
        {
            cornerPositions = gizmosPositions.ManualPositions;

            oppositeCorners = new Vector3[]
            {
                gizmosPositions.TopRightGizmoPosition,      // Opposite of bottom-left
                gizmosPositions.TopLeftGizmoPosition,       // Opposite of bottom-right
                gizmosPositions.BottomLeftGizmoPosition,    // Opposite of top-right
                gizmosPositions.BottomRightGizmoPosition    // Opposite of top-left
            };
        }
        else
        {
            Vector3[] cornerOffsets = CORNER_OFFSETS;

            cornerPositions = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                cornerPositions[i] = Vector3.Scale(cornerOffsets[i], targetBounds.extents);
            }

            oppositeCorners = new Vector3[]
            {
                Vector3.Scale(new Vector3(1, 1, 0), targetBounds.extents),    // Opposite of bottom-left
                Vector3.Scale(new Vector3(-1, 1, 0), targetBounds.extents),   // Opposite of bottom-right
                Vector3.Scale(new Vector3(-1, -1, 0), targetBounds.extents),  // Opposite of top-right
                Vector3.Scale(new Vector3(1, -1, 0), targetBounds.extents)    // Opposite of top-left
            };
        }

        for (int i = 0; i < cornerPositions.Length; i++)
        {
            GameObject gizmoGO = Instantiate(scaleGizmoPrefab, targetTransform);
            gizmoGO.name = $"ScaleGizmo_Corner{i}";

            gizmoGO.transform.localPosition = cornerPositions[i];
            gizmoGO.transform.localRotation = Quaternion.identity;

            TransformGizmo gizmo = gizmoGO.GetComponent<TransformGizmo>();
            gizmo.Initialize(targetTransform, this, TransformGizmo.GizmoType.ScaleCorner);
            gizmo.SetOppositeCorner(oppositeCorners[i]);

            scaleGizmos.Add(gizmo);
        }
    }

    private void CalculateTargetBounds()
    {
        // Calculate bounds from all renderers in the target
        Renderer[] renderers = targetTransform.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            targetBounds = renderers[0].bounds;
            
            foreach (Renderer renderer in renderers)
            {
                targetBounds.Encapsulate(renderer.bounds);
            }

            // Convert to local space
            targetBounds.center = targetTransform.InverseTransformPoint(targetBounds.center);
            targetBounds.size = Vector3.Scale(targetBounds.size, new Vector3(
                1f / targetTransform.lossyScale.x,
                1f / targetTransform.lossyScale.y,
                1f / targetTransform.lossyScale.z
            ));
        }
        else
        {
            // Default bounds
            targetBounds = new Bounds(Vector3.zero, Vector3.one);
        }
    }


    private void UpdateGizmoPositions()
    {
        if (targetTransform == null) 
            return;

        if (useManualGizmoPositions)
        {
            for (int i = 0; i < scaleGizmos.Count && i < gizmosPositions.ManualPositions.Length; i++)
            {
                if (scaleGizmos[i] != null)
                    scaleGizmos[i].transform.localPosition = gizmosPositions.ManualPositions[i];
            }

            if (rotateGizmo != null)
                rotateGizmo.transform.localPosition = gizmosPositions.RotateGizmoPosition;

            if(destroyGizmo != null)
                destroyGizmo.transform.localPosition = gizmosPositions.DestroyGizmoPosition;
        }
        else
        {
            for (int i = 0; i < scaleGizmos.Count && i < CORNER_OFFSETS.Length; i++)
            {
                if (scaleGizmos[i] != null)
                {
                    Vector3 localPos = Vector3.Scale(CORNER_OFFSETS[i], targetBounds.extents);
                    scaleGizmos[i].transform.localPosition = localPos;
                }
            }

            if (rotateGizmo != null)
            {
                float rotateY = targetBounds.extents.y + rotateHandleDistance;
                rotateGizmo.transform.localPosition = new Vector3(0, rotateY, 0);
            }
        }
    }

    public void OnBeginTransform(TransformGizmo.GizmoType type)
    {
        // Callback when transformation starts
    }

    public void OnEndScale(Vector3 startScale, Vector3 endScale)
    {
        if (Vector3.Distance(startScale, endScale) > 0.01f)
        {
            ScaleCommand scaleCommand = new ScaleCommand(targetTransform, startScale, endScale);
            CommandHistory.Instance.ExecuteCommand(scaleCommand);
        }
    }

    public void OnEndRotate(Quaternion startRotation, Quaternion endRotation)
    {
        if (Quaternion.Angle(startRotation, endRotation) > 0.1f)
        {
            RotateCommand rotateCommand = new RotateCommand(targetTransform, startRotation, endRotation);
            CommandHistory.Instance.ExecuteCommand(rotateCommand);
        }
    }


    public void SetGizmosVisible(bool visible)
    {
        gizmosVisible = visible;

        foreach (var gizmo in scaleGizmos)
        {
            if (gizmo != null)
            {
                gizmo.SetVisibility(visible);
            }
        }

        if (rotateGizmo != null)
        {
            rotateGizmo.SetVisibility(visible);
        }

        if(destroyGizmo != null)
        {
            destroyGizmo.SetVisibility(visible);
        }
    }

    public void ClearGizmos()
    {
        foreach (var gizmo in scaleGizmos)
        {
            if (gizmo != null)
            {
                Destroy(gizmo.gameObject);
            }
        }
        scaleGizmos.Clear();

        if (rotateGizmo != null)
        {
            Destroy(rotateGizmo.gameObject);
            rotateGizmo = null;
        }

        if(destroyGizmo != null)
        {
            Destroy(destroyGizmo.gameObject);
            destroyGizmo = null;
        }
    }

    public void ResetTransform()
    {
        if (targetTransform == null) 
            return;

        Vector3 startScale = targetTransform.localScale;
        Quaternion startRotation = targetTransform.rotation;

        targetTransform.localScale = Vector3.one;
        targetTransform.rotation = Quaternion.identity;

        ScaleCommand scaleCmd = new ScaleCommand(targetTransform, startScale, Vector3.one);
        RotateCommand rotateCmd = new RotateCommand(targetTransform, startRotation, Quaternion.identity);

        CommandHistory.Instance.ExecuteCommand(scaleCmd);
        CommandHistory.Instance.ExecuteCommand(rotateCmd);

        UpdateGizmoPositions();
    }


    private void OnDestroy()
    {
        ClearGizmos();
    }

    private void OnDrawGizmos()
    {
        if (targetTransform == null || !gizmosVisible) return;

        Gizmos.color = Color.green;
        Gizmos.matrix = targetTransform.localToWorldMatrix;

        if (!useManualGizmoPositions)
        {
            Gizmos.DrawWireCube(targetBounds.center, targetBounds.size);
        }

        // Draw manual gizmo positions in editor
        if (useManualGizmoPositions && gizmosPositions != null)
        {
            Gizmos.color = Color.cyan;           
            Gizmos.DrawWireSphere(gizmosPositions.BottomLeftGizmoPosition, 0.1f);
            Gizmos.DrawWireSphere(gizmosPositions.BottomRightGizmoPosition, 0.1f);
            Gizmos.DrawWireSphere(gizmosPositions.TopRightGizmoPosition, 0.1f);
            Gizmos.DrawWireSphere(gizmosPositions.TopLeftGizmoPosition, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(gizmosPositions.RotateGizmoPosition, 0.1f);
            Gizmos.DrawWireSphere(gizmosPositions.DestroyGizmoPosition, 0.1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData) => SetGizmosVisible(false);
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ToolStateMachine.CurrentState.ToolType != ToolType.Painting)
            return;

        SetGizmosVisible(true);
    }
}