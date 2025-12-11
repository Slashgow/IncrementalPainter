using TMPro;
using UnityEngine;

public class UISkillPointText : MonoBehaviour
{
    [SerializeField] private TMP_Text skillPointText;
    private void Start()
    {
        SkillPointManager.Instance.OnSkillPointsChanged += UpdateSkillPointText;
        UpdateSkillPointText(SkillPointManager.Instance.CurrentSkillPoints);
    }
    private void OnDestroy()
    {
        if (SkillPointManager.HasInstance)
            SkillPointManager.Instance.OnSkillPointsChanged -= UpdateSkillPointText;
    }
    private void UpdateSkillPointText(int newSkillPoints)
    {
        skillPointText.text = $"{newSkillPoints.ToString()} SP";
    }
}
