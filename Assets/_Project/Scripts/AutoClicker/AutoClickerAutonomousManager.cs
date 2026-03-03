using System.Collections.Generic;
using inkolorgames;
using UnityEngine;

public class AutoClickerAutonomousManager : MonoSingleton<AutoClickerAutonomousManager>
{
    [SerializeField] private AutoClickerDamageor autoClickerDamageor;
    public AutoClickerDamageor AutoClickerDamageor => autoClickerDamageor;

    [SerializeField] private AutoClickerAutonomous autonomousClickerPrefab;
    [SerializeField] private Transform autonomousClickerParent;

    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> autonomousClickerCountPerLevel;
    [SerializeField] private SkillDataPerLevelOfType<FunctionAffine> clickTimerIntervalPerLevel;

    public SkillDataPerLevelOfType<FunctionAffine> AutonomousClickerCountPerLevel => autonomousClickerCountPerLevel;
    public SkillDataPerLevelOfType<FunctionAffine> ClickTimerIntervalPerLevel => clickTimerIntervalPerLevel;
    public int AutonomousClickerCount => Mathf.FloorToInt(autonomousClickerCountPerLevel.GetCurrentLevelData());
    public float ClickTimerInterval => clickTimerIntervalPerLevel.GetCurrentLevelData();

    private List<AutoClickerAutonomous> autonomousClickers = new List<AutoClickerAutonomous>();
    private SpriteRenderer frameRenderer;

    private void Start()
    {
        if (frameRenderer == null)
            frameRenderer = LevelManager.Instance.CurrentLevelInstance.FrameRenderer;

        SpawnAutonomousClickers();

        AutonomousClickerCountPerLevel.OnLevelDown += HandleLevelDown;
        AutonomousClickerCountPerLevel.OnLevelUp += HandleLevelUp; 
    }

    private void OnDestroy()
    {
        AutonomousClickerCountPerLevel.OnLevelDown -= HandleLevelDown;
        AutonomousClickerCountPerLevel.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp()
    {
        autonomousClickers.RemoveAll(autonomousClicker => autonomousClicker == null);

        int autonomousClickerToSpawn = AutonomousClickerCount - autonomousClickers.Count;
        if (autonomousClickerToSpawn > 0)
            SpawnAdditionalAutonomousClicker(autonomousClickerToSpawn);
    }

    private void HandleLevelDown()
    {
        autonomousClickers.RemoveAll(autonomousClicker => autonomousClicker == null);

        int autonomousClickerToDestroy = autonomousClickers.Count - AutonomousClickerCount;
        if (autonomousClickerToDestroy > 0)
        {
            for (int i = 0; i < autonomousClickerToDestroy; i++)
            {
                int lastIndex = autonomousClickers.Count - 1;
                if (lastIndex >= 0)
                {
                    autonomousClickers[lastIndex].OnClick -= AutonomousClicker_OnClick;
                    Destroy(autonomousClickers[lastIndex].gameObject);
                    autonomousClickers.RemoveAt(lastIndex);
                }
            }
        }
    }

    public void SpawnAutonomousClickers()
    {
        for (int i = 0; i < AutonomousClickerCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject autonomousClickerGO = Instantiate(autonomousClickerPrefab.gameObject, spawnPos, Quaternion.identity, autonomousClickerParent);
            autonomousClickerGO.name = $"Autonomous_Clicker_{i + 1}";

            AutoClickerAutonomous autonomousClicker = autonomousClickerGO.GetComponent<AutoClickerAutonomous>();
            autonomousClicker.OnClick += AutonomousClicker_OnClick;
            autonomousClickers.Add(autonomousClicker);
        }
    }

    private void AutonomousClicker_OnClick(Vector3 clickPosition)
    {
        Debug.Log($"Autonomous clicker clicked at position: {clickPosition}");
        autoClickerDamageor.HandleClick(clickPosition);
    }

    public void SpawnAdditionalAutonomousClicker(int additionalCount)
    {
        for (int i = 0; i < additionalCount; i++)
        {
            Vector3 spawnPos = SpriteUtility.GetRandomPositionInSprite(frameRenderer.transform, frameRenderer, true);
            spawnPos.z = 0f;

            GameObject autonomousClickerGO = Instantiate(autonomousClickerPrefab.gameObject, spawnPos, Quaternion.identity, autonomousClickerParent);

            AutoClickerAutonomous autonomousClicker = autonomousClickerGO.GetComponent<AutoClickerAutonomous>();
            autonomousClicker.OnClick += AutonomousClicker_OnClick;
            autonomousClickers.Add(autonomousClicker);
        }
    }
}


