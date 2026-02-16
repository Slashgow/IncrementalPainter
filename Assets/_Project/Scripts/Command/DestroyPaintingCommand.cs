using UnityEngine;

public class DestroyPaintingCommand : ICommand
{
    private GameObject gameObject;
    private LevelData levelData;
    private Vector3 worldPosition;
    private Quaternion worldRotation;
    private Vector3 worldScale;

    public DestroyPaintingCommand(GameObject gameObject, LevelData levelData)
    {
        this.gameObject = gameObject;
        this.levelData = levelData;

        worldPosition = gameObject.transform.position;
        worldRotation = gameObject.transform.rotation;
        worldScale = gameObject.transform.localScale;
    }

    public void Execute() => GameObject.Destroy(gameObject);
    public void Undo()
    {
        GameObject levelGOInstance = GameObject.Instantiate(levelData.LevelPrefab, levelData.SpawnOffset, Quaternion.identity);
        var currentLevelInstance = levelGOInstance.GetComponent<Level>();
        currentLevelInstance.Initialize(levelData, true);

        levelGOInstance.transform.localScale = worldScale;
        levelGOInstance.transform.position = worldPosition;
        levelGOInstance.transform.rotation = worldRotation;
    }
}
