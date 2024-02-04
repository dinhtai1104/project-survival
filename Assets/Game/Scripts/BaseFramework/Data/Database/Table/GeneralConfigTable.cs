using BansheeGz.BGDatabase;

[System.Serializable]
public class GeneralConfigTable : DataTable<string, string>
{
    public override void GetDatabase()
    {
        DB_GeneralConfig.ForEachEntity(e => Get(e));
        DB_EquipmentConfig.ForEachEntity(e => Get(e));
        DB_BuffConfig.ForEachEntity(e => Get(e));
        DB_HeroConfig.ForEachEntity(e => Get(e));
    }

    private void Get(BGEntity e)
    {
        if (e == null) return;
        var configName = e.Get<string>("ConfigName");
        var configValue = e.Get<string>("ConfigValue");

        Dictionary.Add(configName, configValue);
    }

    public string GetValue(string configName,string defaultValue)
    {
        return (!string.IsNullOrEmpty(configName) &&Dictionary.ContainsKey(configName))?Dictionary[configName]:defaultValue;
    }
}