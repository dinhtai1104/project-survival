public class PlayGiftNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        var db = DataManager.Base.PlayGift;
        var save = DataManager.Save.PlayGift;

        foreach (var entity in db.Dictionary)
        {
            if (save.CanClaimedPlay(entity.Value.Id) && !save.IsClaimedPlay(entity.Value.Id))
            {
                return true;
            }
        }
        return false;
    }
}
