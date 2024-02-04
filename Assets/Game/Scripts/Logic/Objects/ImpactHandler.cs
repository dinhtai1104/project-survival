using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public abstract partial class ImpactHandler : UnityEngine.MonoBehaviour,IImpact
{
    [SerializeField]
    protected  float ExplodeDelay = 0;
 
    public BulletBase Base { get { if (bulletBase == null) { bulletBase = GetComponent<BulletBase>(); } return bulletBase; } set => bulletBase = value; }

    private BulletBase bulletBase;
    [SerializeField]
    protected string impactEffect = "VFX_Enemy2_Impact";

    protected float startTime = 0;



    void OnEnable()
    {
        startTime = Time.time;
    }

    public virtual void SetUp(BulletBase bulletBase)
    {
        Base = bulletBase;
    }

    public abstract void Impact(ITarget target);

    public virtual void ForceImpact() { }
    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (Time.time - startTime < ExplodeDelay) return;
        //Impact(collider.GetComponentInParent<ITarget>());
        DelayImpact(collider).Forget();

        async UniTask DelayImpact(Collider2D collider)
        {
            await UniTask.Yield();
            try {
                Logger.Log("TRIGGER: " + collider.gameObject.name  );
                Logger.Log("TRIGGER: " + collider.gameObject.name + " " + collider.GetComponentInParent<ITarget>().GetTransform().gameObject.name);
            }
            catch{}

            Impact(/*collider.gameObject.layer == 8 ? null : */collider.GetComponentInParent<ITarget>());

        }
    }
    protected void ApplyStatus(ITarget target)
    {
        foreach(var handler in GetComponents<ImpactStatusHandler>())
        {
            handler.Apply(Base.Caster, (ActorBase)target, this);
        }
    }
}
