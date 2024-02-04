using System.Collections.Generic;

[System.Serializable]
public class EquipmentInventory 
{
    private List<EquipableItem> equipableItems;
    public EquipmentInventory()
    {
        equipableItems = new List<EquipableItem>();
    }

    public void RemoveEquipment(int Id)
    {
    }

    public void AddToInventory(EquipableItem item)
    {
        equipableItems.Add(item);
    }
    public void AddToInventory(EquipmentSave save)
    {
        var eq = save.CreateEquipableItem();
        equipableItems.Add(eq);
    }
}