using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class SubscriptionTable : DataTable<int, SubscriptionEntity>
{
    public override void GetDatabase()
    {
        DB_Subscription.ForEachEntity(e => Get(e));
    }

    private void Get(BGEntity e)
    {
        var sub = new SubscriptionEntity(e);
        Dictionary.Add(sub.Id, sub);
    }
}