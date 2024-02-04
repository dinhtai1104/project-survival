using System;

public class MergableNotiCondition : NotifyCondition
{
    private bool hasMerge = false;
    private InventorySave equipmentSave;

    private void OnEnable()
    {
        equipmentSave = DataManager.Save.Inventory;
        equipmentSave.onChangeItemEvent += OnChangeItemEvent;
        hasMerge = ValidateMerge();
    }
    private void OnDisable()
    {
        equipmentSave.onChangeItemEvent -= OnChangeItemEvent;
    }

    private void OnChangeItemEvent(EquipmentSave save)
    {
        hasMerge = ValidateMerge();
    }

    private bool ValidateMerge()
    {
        var count = 0;
        foreach (var equipment in equipmentSave.Saves)
        {
            count = 0;
            foreach (var equipment2 in equipmentSave.Saves)
            {
                if (equipment2 == null || equipment == null || equipment == equipment2) continue;
                if (equipment.IsReadyMerge(equipment2))
                {
                    count++;
                    if (count < 2) continue;
                    return true;
                }
            }
        }
        return false;
    }
    public override bool Validate()
    {
        try 
        {
            return ValidateMerge();
        }
        catch (Exception ex)
        {
        }
        return false;
    }
}
