using TMPro;
using UnityEngine;

public class UIStatAffixHeroUpgradeLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private GameObject lockedObject;
    [SerializeField] private TextMeshProUGUI levelLockTxt;
    [SerializeField] private GameObject hiddenAllHeroTxt;


    public void Set(HeroLevelUpgradeEntity stat)
    {
        var hero = DataManager.Save.User.GetHero(stat.Hero);
        descriptionTxt.text = stat.Reward.ToString();
        levelLockTxt.text = $"Lv {stat.Level}";

        hiddenAllHeroTxt.SetActive(stat.IsUseForAllHero);
        if (hero != null)
        {
            lockedObject.SetActive(stat.Level > hero.Level);
        }
        else
        {
            lockedObject.SetActive(true);
        }
    }
}