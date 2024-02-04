[System.Serializable]
public class BlackSmithController
{
    private BlackSmithUpgradeTable database;
    private PlayerData playerData;
    private EquipmentHandler equipmentHandler;
    private InventorySave inventory;

    public BlackSmithController()
    {
        database = DataManager.Base.BlackSmithUpgrade;
        inventory = DataManager.Save.Inventory;
        playerData = GameSceneManager.Instance.PlayerData;
        equipmentHandler = playerData.EquipmentHandler;
    }
}