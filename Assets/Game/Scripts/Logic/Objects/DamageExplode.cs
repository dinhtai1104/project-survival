using Cysharp.Threading.Tasks;
using Game.Damage;
using Game.GameActor;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class DamageExplode : MonoBehaviour
{
    public string VFX_Explode = "VFX_Train_Explode";
    private float sizeExplode;
    private float dmgExplode;
    private AttackCircleCast2D attackCircleCast;
    private ActorBase Caster;
    //[SerializeField] private ParticleSystem effect;
    [SerializeField] private LayerMask layerMask;
    CancellationTokenSource tokenCanel;
    public void Init(ActorBase caster)
    {
        tokenCanel = new CancellationTokenSource();
        this.Caster = caster;
        attackCircleCast = new AttackCircleCast2D(caster, 10);
        attackCircleCast.OnSuccess -= OnSuccessAttack;
        attackCircleCast.OnSuccess += OnSuccessAttack;
    }

    private void OnDisable()
    {
        tokenCanel.Cancel();
        tokenCanel.Dispose();
        attackCircleCast.OnSuccess -= OnSuccessAttack;
    }
    private void OnSuccessAttack(ActorBase target)
    {
        if (target == Caster /*|| target.GetCharacterType() == Caster.GetCharacterType()*/) return;
        DamageSource damageSource = new DamageSource(Caster, (ActorBase)target, dmgExplode, Caster);
        damageSource._damageSource = EDamageSource.Effect;
        target.GetHit(damageSource, target);
    }

    //private Circle
    public async void Explode()
    {
        Game.Pool.GameObjectSpawner.Instance.Get(VFX_Explode, res =>
        {
            res.GetComponent<Game.Effect.EffectAbstract>().Active(transform.position, sizeExplode);
        });
        attackCircleCast.DealDamage(transform.position, Vector3.zero, sizeExplode, layerMask);
        await UniTask.Delay(3000, cancellationToken: tokenCanel.Token);
        PoolManager.Instance.Despawn(gameObject);
    }

    public void SetDmg(float dmg)
    {
        this.dmgExplode = dmg;
    }

    public void SetSize(float sizeExplodeArea)
    {
        this.sizeExplode = sizeExplodeArea;
        transform.localScale = Vector3.one * sizeExplodeArea;
    }
}