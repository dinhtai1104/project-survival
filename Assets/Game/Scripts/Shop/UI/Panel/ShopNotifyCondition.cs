public class ShopNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        var chestSaves = DataManager.Save.Chest.Saves;
        var chestTable = DataManager.Base.Chest;
        foreach (var chest in chestSaves)
        {
            if (chest.Value.CanOpenFreeAd())
            {
                return true;
            }
            if (chest.Value.CanOpenKey())
            {
                return true;
            }
            //if (chest.Value.CanOpenGem())
            //{
            //    return true;
            //}
        }

        return false;
    }
}