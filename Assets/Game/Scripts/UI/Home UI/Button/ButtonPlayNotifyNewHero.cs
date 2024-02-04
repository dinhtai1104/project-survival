using Assets.Game.Scripts.Utilities;
using UnityEngine.UI;

public class ButtonPlayNotifyNewHero : NotifyCondition
{
    public Image avatarImg;
    public override bool Validate()
    {
        if (DataManager.Save.TryHero.CanTriedHero() == false) return false;

        var recommendHero = DataManager.Save.TryHero.GetRecommendHero();
        if (recommendHero == EHero.None) return false;
        var ava = ResourcesLoader.Instance.GetSprite(AtlasName.Hero, recommendHero.ToString());
        avatarImg.sprite = ava;

        return true;
    }
}
