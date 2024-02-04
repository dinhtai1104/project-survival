using UnityEngine;

public class SharkGunBullet : ExplodeBullet
{
    //private SharkGunWeapon weapon => base.weaponBase as SharkGunWeapon;
    //[SerializeField] private AutoDestroyObject blackHolePrefab;
    //protected override void Impact(Collider2D collision)
    //{
    //    var dmgExplMul = weapon.aoe_DmgExplosionMul.FloatValue;
    //    var sizeExpl = weapon.aoe_RadiusExplosion.FloatValue;
    //    var item = weapon.GetEquipableItem();
    //    if (item.EquipmentRarity >= ERarity.Epic)
    //    {
    //        sizeExpl = weapon.epic_aoe_RadiusExplosion.FloatValue;
    //    }

    //    SetDmgExplodeMul(dmgExplMul);
    //    SetExplodeSize(sizeExpl);
    //    base.Impact(collision);

    //    if (item.EquipmentRarity >= ERarity.Legendary)
    //    {
    //        // Spawn BlackHole
    //        var bl = PoolManager.Instance.Spawn(blackHolePrefab);
    //        bl.transform.position = transform.position;

    //        bl.SetDuration(weapon.legendary_aoe_HoleDuration.FloatValue);
    //    }
    //}
}