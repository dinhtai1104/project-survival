using Game.GameActor.Buff;
using UnityEngine;

public class FlamethrowerBuff : AbstractBuff
{
    public ValueConfigSearch flameAttack_Duration;
    public ValueConfigSearch flameAttack_Tickrate;
    public ValueConfigSearch flameAttack_Size;

    [SerializeField] private FlameThrowerObject flameThrowerPrefab;
    private FlameThrowerObject flameThrowerCurrent;

    public override void Play()
    {
        flameThrowerCurrent = PoolManager.Instance.Spawn(flameThrowerPrefab, ((GunBase)Caster.WeaponHandler.CurrentWeapon).weaponObj.transform);
        flameThrowerCurrent.transform.localPosition = Vector3.zero;
        flameThrowerCurrent.transform.localEulerAngles = Vector3.zero;
        flameThrowerCurrent.SetCaster(Caster);
        flameThrowerCurrent.DurationFlameStatus = new Stat(flameAttack_Duration.FloatValue);
        flameThrowerCurrent.TickRateFlameStatus = new Stat(flameAttack_Tickrate.FloatValue);
        flameThrowerCurrent.IntervalDmg = new Stat(GetValue(StatKey.Cooldown));
        flameThrowerCurrent.DmgMulFlameStatus = new Stat(GetValue(StatKey.DmgStatus));
        flameThrowerPrefab.transform.localScale = new Vector3(flameAttack_Size.FloatValue, 1, 1);
        flameThrowerCurrent.Play();
    }
}