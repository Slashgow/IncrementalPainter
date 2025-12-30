using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject magnetItemPrefab;
    [SerializeField] private NukeItem nukeItemPrefab;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> spawnChanceVacuumableMagnetItemPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SpawnChanceVacuumableMagnetItemPerLevel => spawnChanceVacuumableMagnetItemPerLevel;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> spawnChanceNukeItemPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SpawnChanceNukeItemPerLevel => spawnChanceNukeItemPerLevel;

    public float SpawnChanceVacuumableMagnetItem => spawnChanceVacuumableMagnetItemPerLevel.GetCurrentLevelData();
    public float SpawnChanceNukeItem => spawnChanceNukeItemPerLevel.GetCurrentLevelData();

    private SpriteRenderer spriteRenderer;

    private void OnEnable() => PaintSpawner.OnSpawnPaint += PaintSpawner_OnSpawnPaint;
    private void OnDisable() => PaintSpawner.OnSpawnPaint -= PaintSpawner_OnSpawnPaint;

    private void PaintSpawner_OnSpawnPaint()
    {
        if (LuckUtility.RollLuck(SpawnChanceVacuumableMagnetItem))
            SpawnObject(magnetItemPrefab);

        if(LuckUtility.RollLuck(SpawnChanceNukeItem))
            SpawnObject(nukeItemPrefab.gameObject);
    }

    private void SpawnObject(GameObject prefab)
    {
        if (spriteRenderer == null)
            spriteRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        Vector3 spawnPosition = SpriteUtility.GetRandomPositionInSprite(spriteRenderer.transform, spriteRenderer, true);
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}
