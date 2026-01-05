using UnityEngine;

public class UIPagePaintState : UIPage
{
    [SerializeField] private UIHPBar hpBarBoss;

    private void Start()
    {
        if (LevelManager.Instance.CurrentLevelData.IsBossLevel)
        {
            hpBarBoss.Initialize(LevelManager.Instance.CurrentBossDamageableInstance, 
                LevelManager.Instance.CurrentBossDamageableInstance.GetComponent<IHealable>());
            hpBarBoss.gameObject.SetActive(true);
        }
          
        else
            hpBarBoss.gameObject.SetActive(false);
    }
}
 