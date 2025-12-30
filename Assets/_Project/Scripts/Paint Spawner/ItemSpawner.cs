using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject magnetItemPrefab;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> spawnChanceVacuumableMagnetItemPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> SpawnChanceVacuumableMagnetItemPerLevel => spawnChanceVacuumableMagnetItemPerLevel;

    public float SpawnChanceVacuumableMagnetItem => spawnChanceVacuumableMagnetItemPerLevel.GetCurrentLevelData();

    private SpriteRenderer spriteRenderer;

    private void OnEnable() => PaintSpawner.OnSpawnPaint += PaintSpawner_OnSpawnPaint;
    private void OnDisable() => PaintSpawner.OnSpawnPaint -= PaintSpawner_OnSpawnPaint;

    private void PaintSpawner_OnSpawnPaint()
    {
        if (LuckUtility.RollLuck(SpawnChanceVacuumableMagnetItem))
        {
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        if (spriteRenderer == null)
            spriteRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        Vector3 spawnPosition = SpriteUtility.GetRandomPositionInSprite(spriteRenderer.transform, spriteRenderer, true);
        Instantiate(magnetItemPrefab, spawnPosition, Quaternion.identity);
    }
}
