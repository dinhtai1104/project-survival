using System.Collections.Generic;
using TMPro;

public class UICheatAddItemPanel : UI.Panel
{
    public TMP_InputField inputId;
    public TMP_Dropdown rarity;
    public TMP_InputField levelId;
    public TMP_InputField amount;
    private List<ERarity> rarities =new List<ERarity>();
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
        amount.text = "1";
    }
    public void AddItemOnClicked()
    {
        try
        {
            int amount = this.amount.text.TryGetInt();
            for (int i = 0; i < amount; i++)
            {
                var id = inputId.text;
                var rarityId = rarities[rarity.value];
                var level = int.Parse(levelId.text);
                if (DataManager.Base.Equipment.Dictionary.ContainsKey(id))
                {
                    var eqSave = new EquipmentSave { Id = id, Rarity = rarityId, Level = level, IsEquiped = false };
                    DataManager.Save.Inventory.Add(eqSave);
                    FindObjectOfType<UIInventoryPanel>().ShowInventory();
                }
            }
        }
        catch (System.Exception e)
        {

        }
        Close();
    }
}