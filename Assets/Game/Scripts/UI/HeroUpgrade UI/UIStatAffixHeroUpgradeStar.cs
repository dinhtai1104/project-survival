using TMPro;
using UnityEngine;

public class UIStatAffixHeroUpgradeStar : MonoBehaviour
{
    [SerializeField] private UIStarHolder starHolder;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private GameObject lockedObject;

    public void Set(HeroStarUpgradeEntity stat)
    {
        var hero = DataManager.Save.User.GetHero(stat.Hero);
        starHolder.SetStar(ConstantValue.MaxStarHeroUpgrade);
        starHolder.SetStarsActive(stat.Star);
        //descriptionTxt.text = stat.ToString();
        descriptionTxt.text = I2Localize.GetLocalize("UpgradeHero/Increase Dmg And Hp");

        if (hero != null)
        {
            lockedObject.SetActive(stat.Star > hero.Star);
        }
        else
        {
            lockedObject.SetActive(true);
        }
    }
}