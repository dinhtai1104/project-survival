[System.Serializable]
public class WeaponTable : DataTable<string, WeaponEntity>
{
    public override void GetDatabase()
    {
        DB_Weapon.ForEachEntity(e =>
        {
            var wE = new WeaponEntity(e);
            Dictionary.Add(wE.Id, wE);
        });
    }
}