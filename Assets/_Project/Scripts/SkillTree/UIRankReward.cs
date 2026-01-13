using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRankReward : MonoBehaviour, IColorChanger
{
    [SerializeField] private LevelRank rank;
    [SerializeField] private Sprite rankIcon;

    [SerializeField] private TextMeshProUGUI daysRequiredForRankText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Image rankImage;


    private LevelData currentLevelData;

    private void OnEnable() => ThemeColorManager.OnThemeChanged += OnThemeChanged;
    private void OnDisable() => ThemeColorManager.OnThemeChanged -= OnThemeChanged;

    public void OnThemeChanged(ColorTheme newTheme) => UpdateColor();
    public void UpdateColor() => rankImage.color = LevelManager.Instance.GetColorForRank(rank);
    private void Start()
    {
        Initialize(LevelManager.Instance.CurrentLevelData);
    }
    private void OnLevelStart(Level level) => Initialize(LevelManager.Instance.CurrentLevelData);

    public void Initialize(LevelData levelData)
    {
        currentLevelData = levelData;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (currentLevelData == null)
            return;
        
        rankImage.sprite = rankIcon;
        UpdateColor();

        int daysRequired = currentLevelData.GetDaysRequiredForRank(rank);
        if (daysRequired == int.MaxValue)
            daysRequiredForRankText.text = $"> {currentLevelData.GetDaysRequiredForRank(LevelRank.C)} days";
        else
            daysRequiredForRankText.text = $" <= {daysRequired} days";
        
        int skillPoints = currentLevelData.GetSkillPointReward(rank);
        int currency = currentLevelData.GetCurrencyReward(rank);

        if (skillPoints > 0 && currency > 0)
            rewardText.text = $"{skillPoints} SP + {currency} $";
        else if (skillPoints > 0)
            rewardText.text = $"{skillPoints} Skill Points";
        else if (currency > 0)
            rewardText.text = $"{currency} $";
        else
            rewardText.text = "No Reward";
    }


}
