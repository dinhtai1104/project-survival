using Game.GameActor;

public abstract class WeaponPassive : BaseEquipmentPassive
{
    private WeaponEntity entity;
    public WeaponBase Weapon => Caster.WeaponHandler.CurrentWeapon;
    public WeaponEntity WpEntity => entity;
    public override void SetEquipment(EquipableItem itemEquipment)
    {
        this.entity = DataManager.Base.Weapon.Dictionary[itemEquipment.Id];
        base.SetEquipment(itemEquipment);
    }
}