using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class TurretKillManager : MonoSingleton<TurretKillManager>
{
    [SerializeField] private TurretDamageor turretDamageor;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> turretCountPerLevel;
    [SerializeField] private TurretDamageInstance turretPrefab;
 
    public SkillDataPerLevelOfType<FunctionAffine> TurretCountPerLevel => turretCountPerLevel;
    public int CurrentTurretCount => Mathf.RoundToInt(turretCountPerLevel.GetCurrentLevelData());

    private List<TurretDamageInstance> activeTurrets = new List<TurretDamageInstance>();

    private SpriteRenderer frameRenderer;
    void Start()
    {
        if (frameRenderer == null)
            frameRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        SpawnTurrets();

        turretCountPerLevel.OnLevelUp += OnLevelUpTurretCount;
        turretCountPerLevel.OnLevelDown += OnLevelDownTurretCount;
    }

    private void OnDestroy()
    {
        turretCountPerLevel.OnLevelDown -= OnLevelUpTurretCount;
        turretCountPerLevel.OnLevelUp -= OnLevelDownTurretCount;
    }
    private void OnLevelDownTurretCount()
    {
        activeTurrets.RemoveAll(magneter => magneter == null);

        int magneterToDestroy = activeTurrets.Count - CurrentTurretCount;
        if (magneterToDestroy > 0)
        {
            for (int i = 0; i < magneterToDestroy; i++)
            {
                int lastIndex = activeTurrets.Count - 1;
                if (lastIndex >= 0)
                {
                    Destroy(activeTurrets[lastIndex].gameObject);
                    activeTurrets.RemoveAt(lastIndex);
                }
            }
        }
    }

    private void OnLevelUpTurretCount()
    {
        activeTurrets.RemoveAll(turret => turret == null);

        int turretToSpawn = CurrentTurretCount - activeTurrets.Count;
        if (turretToSpawn > 0)
            SpawnAdditionalTurrets(turretToSpawn);
    }

    public void SpawnTurrets()
    {
        for (int i = 0; i < CurrentTurretCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject turretObj = Instantiate(turretPrefab.gameObject, spawnPos, Quaternion.identity, this.transform);
            turretObj.name = $"Turret_Kill_{i + 1}";

            TurretDamageInstance turret = turretObj.GetComponent<TurretDamageInstance>();
            turret.Initialize(turretDamageor);
            activeTurrets.Add(turret);
        }
    }

    public void SpawnAdditionalTurrets(int additionalCount)
    {
        for (int i = 0; i < additionalCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject turretObj = Instantiate(turretPrefab.gameObject, spawnPos, Quaternion.identity, this.transform);

            TurretDamageInstance turret = turretObj.GetComponent<TurretDamageInstance>();
            turret.Initialize(turretDamageor);
            activeTurrets.Add(turret);
        }
    }
}
