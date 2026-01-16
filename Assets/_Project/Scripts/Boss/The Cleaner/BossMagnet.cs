using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

[RequireComponent(typeof(SimpleDamageable))]
public class BossMagnet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SimpleDamageable damageable;

    [Header("Boss Magnet Configuration")]
    [SerializeField] private MagnetData normalMagnetData;
    [SerializeField] private MagnetData enragedMagnetData;
    [SerializeField] private Magneter magneterPrefab;

    [Header("Phase 2 Settings")]
    [SerializeField][Range(0f, 1f)] private float phase2HealthThreshold = 0.5f;
    [SerializeField, Range(0f,10f)] private float positionSwitchTimeInterval = 3f;

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

    private Magneter magnet1;
    private Magneter magnet2;
    private bool isPhase2 = false;
    private Timer positionSwitchTimer;
    private Timer healTimer;
    private int currentPositionIndex = 0; // 0 = position1, 1 = position2

    private void OnEnable()
    {
        damageable.OnTakeDamage += CheckPhaseTransition;
        LevelManager.OnMidLevel += EnterPhase2;
    }

    private void OnDisable()
    {
        damageable.OnTakeDamage -= CheckPhaseTransition;
        LevelManager.OnMidLevel -= EnterPhase2;
    }

    private void Start()
    {
        SpawnFirstMagnet();
        StartHealTimer();
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

    private void CheckPhaseTransition(float currentHealth)
    {
        if (isPhase2)
            return;

        float healthPercentage = currentHealth / damageable.MaxHealth;

        if (healthPercentage <= phase2HealthThreshold)
            EnterPhase2();
    }

    private void EnterPhase2()
    {
        if (isPhase2)
            return;

        isPhase2 = true;
        //Debug.Log("Boss entering Phase 2!");

        if (magnet1 != null)
            magnet1.SetCustomMagnetData(enragedMagnetData);

        magnet2 = SpawnMagneter(magnetPosition2, magnetRotation2, "magneter_2", enragedMagnetData);

        StartPositionSwitching();
    }

    private void StartPositionSwitching() => positionSwitchTimer = Timer.Register(positionSwitchTimeInterval, onComplete: SwitchPosition, isLooped: true);

    private void SwitchPosition()
    {
        if (!isPhase2)
            return;

        currentPositionIndex = (currentPositionIndex + 1) % 2;
        transform.position = currentPositionIndex == 0 ? bossPosition1 : bossPosition2;
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