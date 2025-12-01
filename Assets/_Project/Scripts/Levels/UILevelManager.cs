using TMPro;
using UnityEngine;

public class UILevelManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textCurrentLevelTitle;
    [SerializeField] private TextMeshProUGUI textCurrentLevelAuthor;
    [SerializeField] private TextMeshProUGUI textCurrentLevelArtMovement;

    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject uiLevelPrefab;

    private void Start()
    {
        InitializeLevelCards();
        UpdateCurrentLevelDisplay();
        SubscribeToLevelSelections();
    }

    private void InitializeLevelCards()
    {
        Level[] levels = LevelManager.Instance.SortedLevels;

        foreach (Level level in levels)
        {
            GameObject uiLevelGOInstance = Instantiate(uiLevelPrefab, contentParent);
            UILevel uiLevelInstance = uiLevelGOInstance.GetComponent<UILevel>();

            if (uiLevelInstance != null)
            {
                uiLevelInstance.Initialize(level);
            }
        }
    }

    private void SubscribeToLevelSelections()
    {
        UILevel[] uiLevels = contentParent.GetComponentsInChildren<UILevel>();

        foreach (UILevel uiLevel in uiLevels)
        {
            uiLevel.OnSelectEvent += OnLevelSelected;
        }
    }

    private void OnLevelSelected(Level level)
    {
        UpdateCurrentLevelDisplay();
    }

    private void UpdateCurrentLevelDisplay()
    {
        Level currentLevel = LevelManager.Instance.CurrentLevel;

        if (currentLevel == null) 
            return;

        textCurrentLevelTitle.text = currentLevel.LevelTitle;
        textCurrentLevelAuthor.text = $"{currentLevel.LevelAuthor} - {currentLevel.LevelDate}";
        textCurrentLevelArtMovement.text = currentLevel.ArtMovement.ToString();
    }

    private void OnDestroy()
    {
        UILevel[] uiLevels = contentParent.GetComponentsInChildren<UILevel>();

        foreach (UILevel uiLevel in uiLevels)
        {
            uiLevel.OnSelectEvent -= OnLevelSelected;
        }
    }
}