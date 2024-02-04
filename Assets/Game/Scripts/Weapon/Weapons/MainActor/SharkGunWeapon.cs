using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun/Player/SharkGun")]
public class SharkGunWeapon : GunBase
{
    public ValueConfigSearch aoe_RadiusExplosion;
    public ValueConfigSearch aoe_DmgExplosionMul;

    public ValueConfigSearch epic_aoe_RadiusExplosion;
    public ValueConfigSearch legendary_aoe_HoleDuration;

    public async override UniTask<WeaponBase> SetUp(Character character)
    {
        var ins = (SharkGunWeapon)await base.SetUp(character);
        ins.aoe_DmgExplosionMul = aoe_DmgExplosionMul;
        ins.aoe_RadiusExplosion = aoe_RadiusExplosion;
        ins.epic_aoe_RadiusExplosion = epic_aoe_RadiusExplosion;
        ins.legendary_aoe_HoleDuration = legendary_aoe_HoleDuration;
        return ins;
    }

    public override void Release()
    {
        base.Release();
    }
}