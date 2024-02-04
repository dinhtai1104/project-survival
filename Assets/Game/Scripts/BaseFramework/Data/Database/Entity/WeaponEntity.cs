[System.Serializable]
public class WeaponEntity
{
    public EquipmentEntity equipmentEntity;
    public string Id;
    public float FireRate;
    public float BulletVelocity;
    public float OutputDmg;

    public WeaponEntity(DB_Weapon db)
    {
        Id = db.Get<string>("Id");
        equipmentEntity = DataManager.Base.Equipment.Dictionary[Id];
        
        FireRate = db.Get<float>("FireRate");
        BulletVelocity = db.Get<float>("BulletVelocity");

        OutputDmg = db.Get<float>("OutputDmg");
    }
}