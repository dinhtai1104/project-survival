using System;

[System.Serializable]
public class SubscriptionSave
{
    public int Id;
    public ESubscription Type;
    public bool IsActived;
    public DateTime TimeEnd;

    [NonSerialized] public System.Action Save;
    public SubscriptionSave(int Id, ESubscription Type)
    {
        this.Id = Id;
        this.Type = Type;
        this.IsActived = false;
    }
    public void Active()
    {
        if (IsActived) return;
        IsActived = true;
        var db = DataManager.Base.Subscription.Get(Id);
        TimeEnd = DateTime.UtcNow + new TimeSpan(0, 0, db.Time);
        Save?.Invoke();
        DataManager.Save.Subscription.ApplySubscription();
    }

    public void Deactive()
    {
        IsActived = false;
        Save?.Invoke();
        DataManager.Save.Subscription.ApplySubscription();
    }

    public void Check()
    {
        var left = TimeEnd - DateTime.Now;
        if (left.TotalSeconds <= 0)
        {
            Deactive();
        }
    }
}