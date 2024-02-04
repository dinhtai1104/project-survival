using System.Collections.Generic;
using System.Linq;
using TMPro;

public class UICheatAllItemPanel : UI.Panel
{
    public TMP_Dropdown rarity;
    private List<ERarity> rarities = new List<ERarity>();
    public override void PostInit()
    {
        rarities = new List<ERarity>();
        rarity.ClearOptions();
        var options = new List<string>();
        foreach (var rar in (ERarity[])System.Enum.GetValues(typeof(ERarity)))
        {
            rarities.Add(rar);
            options.Add(rar.ToString());
        }
        rarity.AddOptions(options);
    }
    public void AddItemOnClicked()
    {
        try
        {
            var db = DataManager.Base.Equipment.Dictionary.Values.ToList();
            for (int i = 0; i < db.Count; i++)
            {
                var equip = db[i];
                var rarityId = rarities[rarity.value];
                var eqSave = new EquipmentSave { Id = equip.Id, Rarity = rarityId, Level = 0, IsEquiped = false };
                DataManager.Save.Inventory.Add(eqSave);
            }
            FindObjectOfType<UIInventoryPanel>().ShowInventory();
        }
        catch (System.Exception e)
        {

        }
        Close();
    }

}
