using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private SkillDataBase skillDataBase;
    public SkillDataBase SkillDataBase => skillDataBase;

    [Header("Level Configuration")]
    [SerializeField, Range(0, 10)] private int targetLevel = 1;
    public int TargetLevel => targetLevel;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button button;

    [Header("Connection Lines")]
    [SerializeField] private List<LineRenderer> connectionLines;
    public List<LineRenderer> ConnectionLines => connectionLines;

    private SkillTreeManager treeManager;
    public SkillTreeManager TreeManager => treeManager;

    private SkillState currentState;
    public SkillState CurrentState => currentState;

    private SkillNodeDetail activeDetailInstance;

    public enum SkillState
    {
        Locked,
        Available,
        Unlocked
    }

    private void Awake()
    {
        treeManager = GetComponentInParent<SkillTreeManager>();
        button.onClick.AddListener(OnSkillClicked);
  
    }
    private void Start()
    {
        Initialize();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (treeManager.DetailPrefab != null && treeManager.DetailCanvas != null)
        {
            activeDetailInstance = Instantiate(treeManager.DetailPrefab, 
                (Vector2)treeManager.GetScreenPosition(this.transform.position) + treeManager.DetailOffset, 
                Quaternion.identity, 
                treeManager.DetailCanvas);
            activeDetailInstance.Initialize(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (activeDetailInstance != null)
        {
            Destroy(activeDetailInstance.gameObject);
            activeDetailInstance = null;
        }
    }

    public void Initialize()
    {
        if (iconImage) 
            iconImage.sprite = skillDataBase.Icon;

        UpdateVisuals();
    }

    void OnSkillClicked()
    {
        if (treeManager != null)
        {
            treeManager.TryLevelUpSkillToLevel(skillDataBase, targetLevel);
        }
    }

    public void UpdateVisuals()
    {
        if (skillDataBase == null) 
            return;

        currentState = DetermineState();

        switch (currentState)
        {
            case SkillState.Locked:
                if (backgroundImage) 
                    backgroundImage.sprite = treeManager.SkillTreeVisualData.BackgroundSpriteLocked;
                    backgroundImage.color = treeManager.SkillTreeVisualData.LockedColor;
                if (button) 
                    button.interactable = false;
                break;

            case SkillState.Available:
                if (backgroundImage) 
                    backgroundImage.color = treeManager.SkillTreeVisualData.AvailableColor;
                if (button) 
                    button.interactable = true;
                break;

            case SkillState.Unlocked:
                if (backgroundImage)
                    backgroundImage.sprite = treeManager.SkillTreeVisualData.BackgroundSpriteUnlocked;
                backgroundImage.color = treeManager.SkillTreeVisualData.UnlockedColor;
                if (button) 
                    button.interactable = false;
                break;
        }

        UpdateConnectionLines();
    }

    private SkillState DetermineState()
    {
        int currentLevel = treeManager.GetSkillLevel(skillDataBase.SkillID);

        if (currentLevel >= targetLevel)
            return SkillState.Unlocked;

        if (treeManager.CanLevelUpToLevel(skillDataBase, targetLevel))
            return SkillState.Available;

        return SkillState.Locked;
    }

    void UpdateConnectionLines()
    {
        foreach (var line in connectionLines)
        {
            if (line != null)
            {
                Color lineColor = currentState == SkillState.Unlocked ? treeManager.SkillTreeVisualData.UnlockedColorDarker : 
                    treeManager.SkillTreeVisualData.LockedColorDarker;
                line.startColor = lineColor;
                line.endColor = lineColor;
            }
        }
    }

#if UNITY_EDITOR
    public void CreateConnectionLine(SkillNode targetNode)
    {
        GameObject lineObj = new GameObject($"Line_{skillDataBase.SkillName}_to_{targetNode.skillDataBase.SkillName}");
        lineObj.transform.SetParent(transform.parent);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 3f;
        line.endWidth = 3f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = -1;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetNode.transform.position);

        connectionLines.Add(line);
    }

    [ContextMenu("Create Connections to Required Skills")]
    public void CreateConnectionsToRequiredSkills()
    {
        ClearConnectionLines();

        if (treeManager == null)
            treeManager = GetComponentInParent<SkillTreeManager>();

        if (treeManager == null)
        {
            Debug.LogError("SkillTreeManager not found!");
            return;
        }

        //if (targetLevel > skillDataBase.StartingLevel)
        //{
        //    SkillNode previousLevelNode = treeManager.FindNodeBySkillDataAndLevel(skillDataBase, targetLevel - 1);
        //    if (previousLevelNode != null)
        //    {
        //        CreateConnectionLine(previousLevelNode);
        //        Debug.Log($"Connected {skillDataBase.SkillName} Level {targetLevel} to Level {targetLevel - 1}");
        //    }
        //    else
        //    {
        //        Debug.LogWarning($"Could not find previous level node: {skillDataBase.SkillName} Level {targetLevel - 1}");
        //    }
        //}

        // Connect to required skills for this level
        var levelReq = skillDataBase?.GetRequirementsForLevel(targetLevel);

        if (levelReq != null && levelReq.RequiredSkills != null && levelReq.RequiredSkills.Count > 0)
        {
            foreach (var requiredSkillWithLevel in levelReq.RequiredSkills)
            {
                if (requiredSkillWithLevel.SkillData != null)
                {
                    // Find the node with the specific required level
                    SkillNode requiredNode = treeManager.FindNodeBySkillDataAndLevel(requiredSkillWithLevel.SkillData, requiredSkillWithLevel.RequiredLevel);
                    if (requiredNode != null)
                    {
                        CreateConnectionLine(requiredNode);
                        Debug.Log($"Connected {skillDataBase.SkillName} Level {targetLevel} to required skill: {requiredSkillWithLevel.SkillData.SkillName} Level {requiredSkillWithLevel.RequiredLevel}");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find node for required skill: {requiredSkillWithLevel.SkillData.SkillName} Level {requiredSkillWithLevel.RequiredLevel}");
                    }
                }
            }
        }

        if (connectionLines.Count == 0)
        {
            Debug.Log($"{skillDataBase?.SkillName ?? "Skill"} Level {targetLevel} has no connections to create");
        }
    }

    [ContextMenu("Clear Connection Lines")]
    public void ClearConnectionLines()
    {
        foreach (var line in connectionLines)
        {
            if (line != null)
                DestroyImmediate(line.gameObject);
        }
        connectionLines.Clear();
    }

#endif
}
