using Foundation.Game.Time;
using System;

[System.Serializable]
public class HotSaleHeroSave
{
    public int Id;
    public bool IsClaimed;
    public bool IsNew = true;

    public EHero Hero;
    public DateTime TimeEnd;
    public bool IsActived;

    [NonSerialized]
    public System.Action Save;
    private HotSaleHeroSaves saves => DataManager.Save.HotSaleHero;
    public HotSaleHeroSave(int Id, EHero hero)
    {
        this.Id = Id;
        this.Hero = hero;
        this.IsClaimed = false;
        this.IsActived = false;
        this.IsNew = true;
    }

    public void ActiveHotSale(TimeSpan duration)
    {
        if (IsActived) return;
        this.IsActived = true;
        this.TimeEnd = UnbiasedTime.UtcNow + duration;
        Save?.Invoke();
    }
    public void DeActiveHotSale()
    {
        if (IsNew) IsNew = false;
        this.IsActived = false;
        Save();
    }
}