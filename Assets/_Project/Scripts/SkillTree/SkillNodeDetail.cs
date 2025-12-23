using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNodeDetail : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image backgroundTitleImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI statValueDescriptionText;

    private SkillNode skillNode;
    private SkillTreeManager treeManager;
    private ISkillLevelData skillLevelData;

    private string valueHexaColorLocked;
    private string valueHexaColorAvailable;
    private string valueHexaUnlocked;
    private string valueHexaSkillPointColor;
    private string valueHexaMaxLevelColor;

    private string currentHexaColor;
    private Color currentColorBackground;
    private Color currentColorBackgroundDarker;


    public void Initialize(SkillNode node)
    {
        skillNode = node;
        treeManager = node.TreeManager;

        valueHexaColorAvailable = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.AvailableColorDarker);
        valueHexaUnlocked = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.UnlockedColorDarker);
        valueHexaColorLocked = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.LockedColorDarker);
        valueHexaSkillPointColor = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.SkillPointColor);
        valueHexaMaxLevelColor = ColorUtility.ToHtmlStringRGB(skillNode.TreeManager.SkillTreeVisualData.MaxLevelColor);

        if (skillNode == null || skillNode.SkillDataBase == null)
            return;

        skillLevelData = treeManager.GetSkillLevelData(skillNode.SkillDataBase.SkillID);
        skillLevelData.OnLevelUp += SkillLevelData_OnLevelUp;

        UpdateVisual(skillLevelData);
    }

    private void OnDisable() => skillLevelData.OnLevelUp -= SkillLevelData_OnLevelUp;
    private void SkillLevelData_OnLevelUp() => UpdateVisual(skillLevelData);

    public void UpdateVisual(ISkillLevelData skillLevelData)
    {
        int currentLevel = skillLevelData.CurrentLevel;
        int maxLevel = skillNode.SkillDataBase.MaxLevel;
        bool isMaxed = currentLevel >= maxLevel;

        UpdateCurrentColor();

        if (descriptionText)
            descriptionText.text = skillNode.SkillDataBase.Description.GetLocalizedString();

        if (currentLevelText)
            currentLevelText.text = $"Level <color=#{currentHexaColor}>{currentLevel}</color> / {maxLevel}";

        if (statValueDescriptionText)
        {
            statValueDescriptionText.text = $"<color=#{currentHexaColor}>" +
                $"{FormatUtility.FormatValue(skillNode.SkillDataBase.UnitValue, skillNode.SkillDataBase.GetEffectValueAtLevel(currentLevel))}" +
                $"</color> -> " +
                $"<color=#{currentHexaColor}>" +
                $"{FormatUtility.FormatValue(skillNode.SkillDataBase.UnitValue, skillNode.SkillDataBase.GetEffectValueAtLevel(currentLevel + 1))}" +
                $"</color>";
        }

        if (costText)
        {
            if (isMaxed)
                costText.text = $"<color=#{valueHexaMaxLevelColor}>LVL MAX</color>";
            else
            {
                int cost = skillNode.SkillDataBase.GetCostForLevel(currentLevel + 1);
                int skillPointCost = skillNode.SkillDataBase.GetSkillPointCostForLevel(currentLevel + 1);

                string costStr = $"<color=#{currentHexaColor}>{FormatUtility.FormatValue(cost)} $</color>";
                if (skillPointCost > 0)
                    costStr += $" + <color=#{valueHexaSkillPointColor}>{skillPointCost} SP</color>";

                costText.text = costStr;
            }
        }

        backgroundImage.color = currentColorBackground;
        backgroundTitleImage.color = currentColorBackgroundDarker;
    }

    private void UpdateCurrentColor()
    {
        switch (skillNode.CurrentState)
        {
            case SkillNode.SkillState.Locked:
                currentHexaColor = valueHexaColorLocked;
                currentColorBackground = skillNode.TreeManager.SkillTreeVisualData.LockedColor;
                currentColorBackgroundDarker = skillNode.TreeManager.SkillTreeVisualData.LockedColorDarker;
                break;
            case SkillNode.SkillState.Available:
                currentHexaColor = valueHexaColorAvailable;
                currentColorBackground = skillNode.TreeManager.SkillTreeVisualData.AvailableColor;
                currentColorBackgroundDarker = skillNode.TreeManager.SkillTreeVisualData.AvailableColorDarker;
                break;
            case SkillNode.SkillState.Unlocked:
                currentHexaColor = valueHexaUnlocked;
                currentColorBackground = skillNode.TreeManager.SkillTreeVisualData.UnlockedColor;
                currentColorBackgroundDarker = skillNode.TreeManager.SkillTreeVisualData.UnlockedColorDarker;
                break;
            default:
                break;
        }
    }
}
