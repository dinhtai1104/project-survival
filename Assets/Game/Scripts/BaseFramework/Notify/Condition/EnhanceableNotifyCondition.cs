using System;

public class EnhanceableNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        var save = DataManager.Save.Inventory;
        return Validate(save);
    }

    private bool Validate(InventorySave save)
    {
        try
        {
            if (GameSceneManager.Instance.PlayerData == null) return false;
            if (GameSceneManager.Instance.EquipmentFactory == null) return false;

            foreach (var item in save.Saves)
            {
                if (item.IsEquiped == true && GameSceneManager.Instance.EquipmentFactory.CheckEnhance(item) == EEhanceResult.Success)
                {
                    return true;
                }
            }
        }
        catch(Exception ex)
        {
            //Logger.LogError(ex);
        }
        return false;
    }
}