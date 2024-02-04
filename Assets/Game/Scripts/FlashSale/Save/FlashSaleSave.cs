using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using Foundation.Game.Time;
using System;

[System.Serializable]
public class FlashSaleSave
{
    public int Id;
    public EFlashSale Sale;
    public DateTime TimeEnd;
    public bool IsClaimed;
    public bool IsActive;
    public int ShowInDay;
    public bool IsShowed = false;
    public int Count = 0;

    public bool IsEnd => ((IsActive || IsClaimed) && (TimeEnd - UnbiasedTime.UtcNow).TotalSeconds <= 0) && Sale != EFlashSale.StarterPack;
    public FlashSaleSave()
    {
        IsClaimed = false;
        ShowInDay = 0;
        IsShowed = false;
    }

    public FlashSaleSave(int Id, EFlashSale sale) : base()
    {
        this.Id = Id;
        Sale = sale;
        IsActive = false;
        IsShowed = false;
    }

    public void NextDay()
    {
        ShowInDay = 0;
        IsShowed = false;
    }
    public void Active()
    {
        if (IsActive) return;
        var data = DataManager.Base.FlashSale.Get(Id);

        TimeEnd = UnbiasedTime.UtcNow + data.TimeEndAdd;
        IsActive = true;
        DataManager.Save.FlashSale.Save();
    }
    public void Claim()
    {
        Count++;
        IsClaimed = true;
        IsActive = false;
        ShowInDay = 9999;
        IsShowed = false;
        DataManager.Save.FlashSale.Save();
    }

    public bool CanShow()
    {
        var data = DataManager.Base.FlashSale.Get(Id);

        return ShowInDay < data.MaxShowInDay;
    }

    public void Show()
    {
        //ShowInDay++;
        //DataManager.Save.FlashSale.Save();
    }

    public void Deactive()
    {
        IsActive = false;
        IsClaimed = false;
        ShowInDay = 0;
        IsShowed = false;
    }

    public bool IsShow()
    {
        return IsShowed;
    }
}