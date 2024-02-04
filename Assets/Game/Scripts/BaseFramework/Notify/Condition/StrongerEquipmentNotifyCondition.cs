using System;

public class StrongerEquipmentNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        return Validate(DataManager.Save.Inventory);
    }

    private bool Validate(InventorySave inventory)
    {
        try
        {
            if (GameSceneManager.Instance.PlayerData == null) return false;
            var equipHandler = GameSceneManager.Instance.PlayerData.EquipmentHandler;
            if (equipHandler == null) return false;
            foreach (var eq in (EEquipment[])Enum.GetValues(typeof(EEquipment)))
            {
                if (equipHandler.HasEquipmentType(eq))
                {
                    var eqment = equipHandler.GetEquipment(eq);
                    EquipableItem bestEquip = eqment;

                    foreach (var equip in inventory.AllEquipableItem)
                    {
                        //if (equip.IsEquipped) continue;
                        if (equip.EquipmentType != eq) continue;
                        if (bestEquip.BaseStatAffix.Value < equip.BaseStatAffix.Value)
                        {
                            bestEquip = equip;
                        }
                    }
                    if (inventory.AllEquipableItem != null && bestEquip != null && eqment != bestEquip)
                    {
                        //Equip
                        return true;
                    }
                }
                else
                {
                    EquipableItem bestEquip = null;
                    foreach (var equip in inventory.AllEquipableItem)
                    {
                       // if (equip.IsEquipped) continue;
                        if (equip.EquipmentType != eq) continue;
                        if (bestEquip == null)
                        {
                            bestEquip = equip;
                            continue;
                        }
                        if (bestEquip.BaseStatAffix.Value < equip.BaseStatAffix.Value)
                        {
                            bestEquip = equip;
                        }
                    }
                    if (inventory.AllEquipableItem != null && bestEquip != null)
                    {
                        //Equip
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }
        return false;
    }
}