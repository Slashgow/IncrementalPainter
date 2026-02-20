using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class TurretSpawnerManager : MonoSingleton<TurretSpawnerManager>
{
    [SerializeField] private TurretSpawner turretSpawner;
    [SerializeField] private TurretSpawnerInstance turretSpawnerPrefab;

    public TurretSpawner TurretSpawner => turretSpawner;
    public int CurrentTurretCount => turretSpawner.TurretCount;

    private List<TurretSpawnerInstance> activeTurrets = new List<TurretSpawnerInstance>();
    private SpriteRenderer frameRenderer;

    void Start()
    {
        if (frameRenderer == null)
            frameRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        SpawnTurrets();

        turretSpawner.TurretCountSkillDataPerLevel.OnLevelUp += OnLevelUpTurretCount;
        turretSpawner.TurretCountSkillDataPerLevel.OnLevelDown += OnLevelDownTurretCount;
    }

    private void OnDestroy()
    {
        turretSpawner.TurretCountSkillDataPerLevel.OnLevelUp -= OnLevelUpTurretCount;
        turretSpawner.TurretCountSkillDataPerLevel.OnLevelDown -= OnLevelDownTurretCount;
    }

    private void OnLevelUpTurretCount()
    {
        activeTurrets.RemoveAll(t => t == null);

        int toSpawn = CurrentTurretCount - activeTurrets.Count;
        if (toSpawn > 0)
            SpawnAdditionalTurrets(toSpawn);
    }

    private void OnLevelDownTurretCount()
    {
        activeTurrets.RemoveAll(t => t == null);

        int toDestroy = activeTurrets.Count - CurrentTurretCount;
        for (int i = 0; i < toDestroy; i++)
        {
            int lastIndex = activeTurrets.Count - 1;
            if (lastIndex < 0) break;

            Destroy(activeTurrets[lastIndex].gameObject);
            activeTurrets.RemoveAt(lastIndex);
        }
    }

    public void SpawnTurrets()
    {
        for (int i = 0; i < CurrentTurretCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject obj = Instantiate(turretSpawnerPrefab.gameObject, spawnPos, Quaternion.identity, this.transform);
            obj.name = $"TurretSpawner_{i + 1}";

            TurretSpawnerInstance turret = obj.GetComponent<TurretSpawnerInstance>();
            turret.Initialize(turretSpawner);
            activeTurrets.Add(turret);
        }
    }

    public void SpawnAdditionalTurrets(int additionalCount)
    {
        for (int i = 0; i < additionalCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject obj = Instantiate(turretSpawnerPrefab.gameObject, spawnPos, Quaternion.identity, this.transform);

            TurretSpawnerInstance turret = obj.GetComponent<TurretSpawnerInstance>();
            turret.Initialize(turretSpawner);
            activeTurrets.Add(turret);
        }
    }
}