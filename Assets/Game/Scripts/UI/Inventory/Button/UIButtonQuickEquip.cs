using System;

public class UIButtonQuickEquip : UIBaseButton
{
    public override void Action()
    {
        var inventory = DataManager.Save.Inventory;
        if (GameSceneManager.Instance.PlayerData == null) return;
        var equipHandler = GameSceneManager.Instance.PlayerData.EquipmentHandler;
        if (equipHandler == null) return;
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
                if (inventory.AllEquipableItem != null && bestEquip != null && bestEquip != eqment)
                {
                    //Equip
                    equipHandler.Equip(bestEquip);
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
                    equipHandler.Equip(bestEquip);
                }
            }
        }
        Messenger.Broadcast(EventKey.QuickEquip);
    }
}