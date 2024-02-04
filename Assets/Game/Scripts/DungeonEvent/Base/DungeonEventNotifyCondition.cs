using System;

public class DungeonEventNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        try
        {
            var database = DataManager.Base.DungeonEventConfig;
            var datasave = DataManager.Save.DungeonEvent;
            var resource = DataManager.Save.Resources;
            foreach (var item in database.Dictionary)
            {
                if (resource.HasResource(EResource.Energy, item.Value[0].Energy) && datasave.Saves[item.Key].FreeLeftDay > 0)
                {
                    return true;
                }
            }
        }
        catch (Exception e)
        {

        }
        return false;
    }
}
