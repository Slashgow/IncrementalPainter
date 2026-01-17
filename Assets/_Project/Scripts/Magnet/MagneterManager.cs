using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class MagneterManager : MonoSingleton<MagneterManager>
{
    [Header("Magnet Configuration")]
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> attractionForcePerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> attractionRadiusPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> orbitRadiusPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> orbitSpeedPerLevel;

    [Header("Magneter Spawning")]
    [SerializeField] private Magneter magneterPrefab; 
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> magneterCountPerLevel;
    [SerializeField] private Transform magneterParent; 

    public SkillDataPerLevelOfType<FunctionAffine> AttractionForcePerLevel => attractionForcePerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> AttractionRadiusPerLevel => attractionRadiusPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> OrbitRadiusPerLevel => orbitRadiusPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> OrbitSpeedPerLevel => orbitSpeedPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> MagneterCountPerLevel => magneterCountPerLevel;

    public int MagneterCountToSpawn => Mathf.FloorToInt(magneterCountPerLevel.GetCurrentLevelData());

    private List<Magneter> playerMagneters = new List<Magneter>();
    private SpriteRenderer frameRenderer;
    void Start()
    {
        if (frameRenderer == null)
            frameRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        SpawnMagneters();

        MagneterCountPerLevel.OnLevelUp += MagneterManager_OnLevelUp;
        MagneterCountPerLevel.OnLevelDown += MagneterCountPerLevel_OnLevelDown;
    }

    private void OnDestroy()
    {
        MagneterCountPerLevel.OnLevelDown -= MagneterCountPerLevel_OnLevelDown;
        MagneterCountPerLevel.OnLevelUp -= MagneterManager_OnLevelUp;
    }

    private void MagneterCountPerLevel_OnLevelDown()
    {
        playerMagneters.RemoveAll(magneter => magneter == null);

        int magneterToDestroy = playerMagneters.Count - MagneterCountToSpawn;
        if (magneterToDestroy > 0)
        {
            for (int i = 0; i < magneterToDestroy; i++)
            {
                int lastIndex = playerMagneters.Count - 1;
                if (lastIndex >= 0)
                {
                    Destroy(playerMagneters[lastIndex].gameObject);
                    playerMagneters.RemoveAt(lastIndex);
                }
            }
        }
    }


    private void MagneterManager_OnLevelUp()
    {
        playerMagneters.RemoveAll(magneter => magneter == null);

        int magneterToSpawn = MagneterCountToSpawn - playerMagneters.Count;
        if (magneterToSpawn > 0)
            SpawnAdditionalMagneters(magneterToSpawn);
    }

    public void SpawnMagneters()
    {
        for (int i = 0; i < MagneterCountToSpawn; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject magneterObj = Instantiate(magneterPrefab.gameObject, spawnPos, Quaternion.identity, magneterParent);
            magneterObj.name = $"Magneter_{i + 1}";

            Magneter magneter = magneterObj.GetComponent<Magneter>();
            playerMagneters.Add(magneter);
        }
    }

    public void SpawnAdditionalMagneters(int additionalCount)
    {
        for (int i = 0; i < additionalCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject magneterObj = Instantiate(magneterPrefab.gameObject, spawnPos, Quaternion.identity, magneterParent);

            Magneter magneter = magneterObj.GetComponent<Magneter>();
            playerMagneters.Add(magneter);
        }
    }

    public bool IsPlayerMagneter(Magneter magneter) => playerMagneters.Contains(magneter);
}
