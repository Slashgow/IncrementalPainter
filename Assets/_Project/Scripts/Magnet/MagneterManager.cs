using inkolorgames;
using PaintIn2D;
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

    private SpriteRenderer frameRenderer;
    void Start()
    {
        if (frameRenderer == null)
            frameRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        SpawnMagneters();

        MagneterCountPerLevel.OnLevelUp += MagneterManager_OnLevelUp;
    }

    private void OnDestroy() => MagneterCountPerLevel.OnLevelUp -= MagneterManager_OnLevelUp;
    private void MagneterManager_OnLevelUp()
    {
        int currentMagneterCount = FindObjectsByType<Magneter>(FindObjectsSortMode.None).Length;

        int magneterToSpawn = MagneterCountToSpawn - currentMagneterCount;
        if(magneterToSpawn > 0)
            SpawnAdditionalMagneters(magneterToSpawn);
    }

    public void SpawnMagneters()
    {
        for (int i = 0; i < MagneterCountToSpawn; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer);
            spawnPos.z = 0f;

            GameObject magneterObj = Instantiate(magneterPrefab.gameObject, spawnPos, Quaternion.identity, magneterParent);
            magneterObj.name = $"Magneter_{i + 1}";
        }
    }

    public void SpawnAdditionalMagneters(int additionalCount)
    {
        for (int i = 0; i < additionalCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer);
            spawnPos.z = 0f;

            Instantiate(magneterPrefab, spawnPos, Quaternion.identity, magneterParent);
        }
    }
}
