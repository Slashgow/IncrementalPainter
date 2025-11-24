using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNode : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkillDataBase skillDataBase;
    public SkillDataBase SkillDataBase => skillDataBase;

    [Header("Level Configuration")]
    [SerializeField, Range(0, 10)] private int targetLevel = 1;
    public int TargetLevel => targetLevel;

    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button button;

    [Header("Connection Lines")]
    [SerializeField] private List<LineRenderer> connectionLines;
    public List<LineRenderer> ConnectionLines => connectionLines;

    private SkillTreeManager treeManager;
    private SkillState currentState;

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

    public void Initialize()
    {
        if (iconImage) 
            iconImage.sprite = skillDataBase.Icon;
        if (nameText) 
            nameText.text = skillDataBase.SkillName;
        if (levelText)
            levelText.text = $"Level {targetLevel}";

        var levelReq = skillDataBase.GetRequirementsForLevel(targetLevel);
        if (costText && levelReq != null)
        {
            costText.text = $"Cost: {levelReq.SkillPointCost} SP";
            if (levelReq.CurrencyCost > 0)
                costText.text += $"\n{levelReq.CurrencyCost} Gold";
        }
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
                    backgroundImage.color = skillDataBase.LockedColor;
                if (button) 
                    button.interactable = false;
                break;

            case SkillState.Available:
                if (backgroundImage) 
                    backgroundImage.color = skillDataBase.AvailableColor;
                if (button) 
                    button.interactable = true;
                break;

            case SkillState.Unlocked:
                if (backgroundImage) 
                    backgroundImage.color = skillDataBase.UnlockedColor;
                if (button) 
                    button.interactable = false;
                break;
        }

        UpdateConnectionLines();
    }

    SkillState DetermineState()
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
                Color lineColor = currentState == SkillState.Unlocked ? skillDataBase.UnlockedColor : skillDataBase.LockedColor;
                lineColor.a = 0.5f;
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

        if (targetLevel > skillDataBase.StartingLevel)
        {
            SkillNode previousLevelNode = treeManager.FindNodeBySkillDataAndLevel(skillDataBase, targetLevel - 1);
            if (previousLevelNode != null)
            {
                CreateConnectionLine(previousLevelNode);
                Debug.Log($"Connected {skillDataBase.SkillName} Level {targetLevel} to Level {targetLevel - 1}");
            }
            else
            {
                Debug.LogWarning($"Could not find previous level node: {skillDataBase.SkillName} Level {targetLevel - 1}");
            }
        }

        // Connect to required skills for this level
        var levelReq = skillDataBase?.GetRequirementsForLevel(targetLevel);

        if (levelReq != null && levelReq.RequiredSkills != null && levelReq.RequiredSkills.Count > 0)
        {
            foreach (var requiredSkill in levelReq.RequiredSkills)
            {
                if (requiredSkill != null)
                {
                    // Find any node with this skill (preferably the highest level)
                    SkillNode requiredNode = treeManager.FindNodeBySkillData(requiredSkill);
                    if (requiredNode != null)
                    {
                        CreateConnectionLine(requiredNode);
                        Debug.Log($"Connected {skillDataBase.SkillName} Level {targetLevel} to required skill: {requiredSkill.SkillName}");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find node for required skill: {requiredSkill.SkillName}");
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
