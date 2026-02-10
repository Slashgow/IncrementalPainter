using System.Collections.Generic;
using UnityEngine;
using UnityTimer;


[RequireComponent(typeof(SimpleDamageable))]
public class BossMagnet : BossPhased
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Boss Magnet Configuration")]
    [SerializeField] private MagnetData normalMagnetData;
    [SerializeField] private MagnetData enragedMagnetData;
    [SerializeField] private Magneter magneterPrefab;

    [Header("Phase 2 Settings")]
    [SerializeField, Range(0f,10f)] private float positionSwitchTimeInterval = 3f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private AnimationCurve dashCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Boss Positions")]
    [SerializeField] private Vector3 bossPosition1 = new Vector3(-5f, 3f, 0f);
    [SerializeField] private Vector3 bossPosition2 = new Vector3(5f, 3f, 0f);
    [SerializeField] private Vector3 magnetPosition1 = new Vector3(-5f, 3f, 0f);
    [SerializeField] private Vector3 magnetPosition2 = new Vector3(5f, 3f, 0f);
    [SerializeField] private Quaternion magnetRotation1 = Quaternion.identity;
    [SerializeField] private Quaternion magnetRotation2 = Quaternion.identity;

    [Header("Healing Settings")]
    [SerializeField, Range(0f,5f)] private float healRadius = 1.5f;
    [SerializeField, Range(0f,30f)] private float healAmountPerMagnetable = 5f;
    [SerializeField, Range(0f,5f)] private float healCheckInterval = 0.5f;
    [SerializeField] private LayerMask magnetableLayers = ~0;

    [Header("Animation Parameters")]
    private readonly int DASH_TRIGGER_HASH = Animator.StringToHash("Dash");
    private readonly int IDLE_TRIGGER_HASH = Animator.StringToHash("Idle");

    private Magneter magnet1;
    private Magneter magnet2;
    private Timer positionSwitchTimer;
    private Timer healTimer;
    private int currentPositionIndex = 0; // 0 = position1, 1 = position2

    private bool isDashing = false;
    private Vector3 dashStartPosition;
    private Vector3 dashTargetPosition;
    private float dashProgress = 0f;

    private void Start()
    {
        SpawnFirstMagnet();
        StartHealTimer();
    }
    private void Update()
    {
        if (isDashing)
            UpdateDash();
    }

    private Magneter SpawnMagneter(Vector3 position, Quaternion quaternion, string name, MagnetData magnetData)
    {
        GameObject magnetGO = Instantiate(magneterPrefab.gameObject, position, quaternion);
        Magneter magneter = magnetGO.GetComponent<Magneter>();
        magnetGO.name = name;
        magneter.SetCustomMagnetData(magnetData);
        magneter.GetComponent<MouseDraggable>().SetCanDrag(false);
        magneter.SetActive(true);

        return magneter;
    }

    private void SpawnFirstMagnet()
    {
        magnet1 = SpawnMagneter(magnetPosition1, magnetRotation1, "magneter_1", normalMagnetData);
        transform.position = bossPosition1;
    }


    protected override void EnterPhase2()
    {   
        if(isPhase2)
            return;

        if (magnet1 != null)
            magnet1.SetCustomMagnetData(enragedMagnetData);

        magnet2 = SpawnMagneter(magnetPosition2, magnetRotation2, "magneter_2", enragedMagnetData);

        StartPositionSwitching();

        isPhase2 = true;
    }

    private void StartPositionSwitching() => positionSwitchTimer = Timer.Register(positionSwitchTimeInterval, onComplete: SwitchPosition, isLooped: true);

    private void SwitchPosition()
    {
        if (!isPhase2 || isDashing)
            return;

        currentPositionIndex = (currentPositionIndex + 1) % 2;
        Vector3 targetPosition = currentPositionIndex == 0 ? bossPosition1 : bossPosition2;

        StartDash(targetPosition);
    }
    private void StartDash(Vector3 targetPosition)
    {
        isDashing = true;
        dashProgress = 0f;
        dashStartPosition = transform.position;
        dashTargetPosition = targetPosition;
        animator.SetTrigger(DASH_TRIGGER_HASH);
    }

    private void UpdateDash()
    {
        float distance = Vector3.Distance(dashStartPosition, dashTargetPosition);
        if (distance < 0.01f)
        {
            EndDash();
            return;
        }

        dashProgress += (dashSpeed / distance) * Time.deltaTime;

        if (dashProgress >= 1f)
        {
            transform.position = dashTargetPosition;
            EndDash();
        }
        else
        {
            float curvedProgress = dashCurve.Evaluate(dashProgress);
            transform.position = Vector3.Lerp(dashStartPosition, dashTargetPosition, curvedProgress);
        }
    }

    private void EndDash()
    {
        isDashing = false;
        dashProgress = 0f;
        transform.position = dashTargetPosition;
        animator.SetTrigger(IDLE_TRIGGER_HASH);
    }
    private void StartHealTimer()
    {
        healTimer = Timer.Register(healCheckInterval, onComplete: TryHealFromMagnetables, isLooped: true);
    }

    private void TryHealFromMagnetables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, healRadius, magnetableLayers);

        HashSet<IMagnetable> magnetablesInRange = new HashSet<IMagnetable>();

        foreach (var collider in colliders)
        {
            var magnetable = collider.GetComponentInParent<IMagnetable>();
            if (magnetable != null && MagnetUtility.IsValidMagnetable(magnetable))
            {
                magnetablesInRange.Add(magnetable);
            }
        }

        if (magnetablesInRange.Count > 0)
        {
            float totalHealAmount = magnetablesInRange.Count * healAmountPerMagnetable;
            damageable.Heal(totalHealAmount);

            //Debug.Log($"Boss healed {totalHealAmount} HP from {magnetablesInRange.Count} magnetables!");
        }
    }

    public void ActivateMagnets()
    {
        if (magnet1 != null)
            magnet1.SetActive(true);

        if (magnet2 != null)
            magnet2.SetActive(true);
    }

    public void DeactivateMagnets()
    {
        if (magnet1 != null)
            magnet1.SetActive(false);

        if (magnet2 != null)
            magnet2.SetActive(false);
    }

    private void OnDestroy()
    {
        positionSwitchTimer?.Cancel();
        healTimer?.Cancel();

        if (magnet1 != null)
            Destroy(magnet1.gameObject);

        if (magnet2 != null)
            Destroy(magnet2.gameObject);
    }

    private void OnDrawGizmos()
    {
        // Draw magnet positions in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(bossPosition1, 0.5f);
        Gizmos.DrawLine(bossPosition1 + Vector3.up * 0.5f, bossPosition1 - Vector3.up * 0.5f);
        Gizmos.DrawLine(bossPosition1 + Vector3.right * 0.5f, bossPosition1 - Vector3.right * 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(bossPosition2, 0.5f);
        Gizmos.DrawLine(bossPosition2 + Vector3.up * 0.5f, bossPosition2 - Vector3.up * 0.5f);
        Gizmos.DrawLine(bossPosition2 + Vector3.right * 0.5f, bossPosition2 - Vector3.right * 0.5f);

        // Draw line between positions
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(bossPosition1, bossPosition2);

        // Draw current position indicator in play mode
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.7f);

            // Draw heal radius
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, healRadius);
        }
    }
}