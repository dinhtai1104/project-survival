using System.Collections.Generic;

[System.Serializable]
public class EquipementLevelUpTable
{
    public Dictionary<int, EquipmentLevelUpEntity> EquipmentLevelUp = new Dictionary<int, EquipmentLevelUpEntity>();

    public void GetDatabase()
    {

    }
}

[System.Serializable]
public class EquipmentLevelsUpEntity
{
    public List<EquipmentLevelUpEntity> entites = new List<EquipmentLevelUpEntity>();
}
public class EquipmentLevelUpEntity
{

}