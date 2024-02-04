using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu (menuName = "EnemyWeapon/EnemyCloseAttack")]
public class MeleeKnife : WeaponBase,IDamageDealer
{
    [SerializeField]
    AssetReferenceGameObject slashRef;
    [SerializeField]
    private AudioClip[] slashSFXs;
    public override async UniTask<WeaponBase> SetUp(Character character)
    {
        MeleeKnife instance = (MeleeKnife)await base.SetUp(character);

        instance.slashRef = slashRef;
        instance.slashSFXs = slashSFXs;

        return instance;
    }
    RaycastHit2D []hits=new RaycastHit2D[10];
    Transform triggerPos; Vector2 facing; string playerTag;
    //public override bool Trigger(Transform triggerPos, System.Action onEnded)
    //{
    //    if (Time.time - lastTrigger >= 1 / GetFireRate())
    //    {
    //        this.onEnded = onEnded;
    //        this.triggerPos = triggerPos;
    //        this.facing = character.GetLookDirection();
    //        character.AnimationHandler.onTriggerEvent += OnEvent;

    //        // avoid trigger again until end of attack animation
    //        lastTrigger = 9999999;
    //        return true;
    //    }
    //    return false;
    //}
    public override bool Trigger(Transform triggerPos,Transform target, Vector2 facing, ITarget trackingTarget, System.Action onEnded)
    {
        if (Time.time - lastTrigger >= 1 / GetFireRate())
        {
            this.onEnded = onEnded;
            this.triggerPos = triggerPos;
            this.facing = facing;
            character.AnimationHandler.onTriggerEvent += OnEvent;

            // avoid trigger again until end of attack animation
            lastTrigger = 9999999;
            return true;
        }
        return false;
    }
    //public override bool Trigger(Transform triggerPos, ITarget target, System.Action onEnded)
    //{
    //    if (Time.time - lastTrigger >= 1 / GetFireRate())
    //    {
    //        this.onEnded = onEnded;
    //        this.triggerPos = triggerPos;
    //        this.facing.Set(target.GetPosition().x>character.GetPosition().x?1:-1,0);
    //        character.AnimationHandler.onTriggerEvent += OnEvent;

    //        // avoid trigger again until end of attack animation
    //        lastTrigger = 9999999;
    //        return true;
    //    }
    //    return false;
    //}
    void OnEvent(Spine.Event e)
    {
        character.AnimationHandler.onTriggerEvent -= OnEvent;
        if(e.Data.Name.Equals("attack_tracking"))
        {
            Sound.Controller.Instance.PlayOneShot(slashSFXs[Random.Range(0, slashSFXs.Length)]);
            int count = Physics2D.CircleCastNonAlloc(triggerPos.position, range, facing, hits, range, mask);
            if (count > 0)
            {

                Game.Pool.GameObjectSpawner.Instance.Get(slashRef.RuntimeKey.ToString(), obj =>
                {
                    obj.GetComponent<Game.Effect.EffectAbstract>().Active(triggerPos.position);
                });
            }
            for (int i = 0; i < count; i++)
            {
                RaycastHit2D hit = hits[i];
                if (hit.collider != null )
                {
                    ITarget target = hit.collider.GetComponentInParent<ITarget>();
                    if (target != null && (Object)target != character && !target.IsDead())
                    {
                        DamageSource damageSource = new DamageSource(character, (ActorBase)target, this.GetDamage(),this);
                        damageSource._damageSource = EDamageSource.Weapon;
                        target.GetHit(damageSource, this);
                    }
                }
            }
            lastTrigger = Time.time;
            OnAttackEnded();
        }
    }
    public override void Destroy()
    {
        base.Destroy();
        if(character!=null)
        character.AnimationHandler.onTriggerEvent -= OnEvent;
    }
    public Transform GetTransform()
    {
        return character.WeaponHandler.GetAttackPoint(this);
    }
    public Vector3 GetDamagePosition()
    {
        return GetTransform().position;
    }
}
