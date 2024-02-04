using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/Trigger Weapon When Get Hit")]
public class TriggerWeaponWhenGetHit : CharacterBehaviour
{
    public WeaponBase weaponBase;
    private WeaponBase weaponInstance;
    public ValueConfigSearch CoolDown;

    float time = 0;

    public override CharacterBehaviour SetUp(ActorBase character)
    {
        TriggerWeaponWhenGetHit instance = (TriggerWeaponWhenGetHit)base.SetUp(character);
        instance.weaponBase = weaponBase;
        instance.CoolDown = CoolDown.SetId(character.gameObject.name);
        weaponBase.SetUp((Character)character).ContinueWith(weapon=> instance.weaponInstance=weapon).Forget();
        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
        time = Time.time;
    }
    public override void OnGetHit(ActorBase character)
    {
        if (Time.time - time > CoolDown.FloatValue)
        {
            weaponInstance.Trigger(character.GetMidTransform(), null, Vector2.up, character.Sensor.CurrentTarget,null);
            time = Time.time;
        }
    }
}
