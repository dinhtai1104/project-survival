using Cysharp.Threading.Tasks;
using Effect;
using Game.Effect;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnVisible
{
    void OnVisible(bool isVisible);
}
public interface IDamageDealer
{
    Transform GetTransform();
    Vector3 GetDamagePosition();
}
public class BulletBase : MonoBehaviour,IOnVisible,IDamageDealer, IDamage
{
     //event
    public delegate void OnBulletDeactive(BulletBase bullet, ActorBase actor);
    public static OnBulletDeactive onBulletDeactive;
    public InvokeHitByTrigger.OnTrigger onTrigger;

    //properties
    public ActorBase Caster
    {
        get => weaponBase.character;
        set { }
    }
    public Stat DmgStat { set; get; }

    protected Transform _transform;
    [HideInInspector]
    public WeaponBase weaponBase;

    protected List<IImpact> impactHandlers;
    protected IMove moveHandler;


    public IMove MoveHandler { get { if (moveHandler == null) moveHandler = GetComponent<IMove>();  return moveHandler; } }
    public List<IImpact> ImpactHandlers { get { if (impactHandlers == null) impactHandlers = new List<IImpact>(GetComponents<IImpact>());  return impactHandlers; } }
    public float Velocity = 10,bounceRadius=4,Size=1;
    public int MaxPiercing = 1;
    public int MaxBounce=0;
    public int MaxRicochet=0;
    public float TrackingBulletLevel=0;

    private int defaultMaxPiercing,defaultMaxBounce,defaultMaxRicochet;
    private float defaultTrackingLevel;
    public bool destroyOnImpact = true;

    [HideInInspector]
    public LayerMask mask;
    private void OnDisable()
    {
    }
    private  void OnDestroy()
    {
       
    }
    //[HideInInspector]
    public int currentBounce=0,currentPiercing=0,currentRicochet=0;
    public bool firstImpact = false,canRicochet=false;

    private Vector3 defaultScale;

    List<ModifierSource> modifierSources;
    public virtual void SetUp(WeaponBase weaponBase,Vector2 triggerPos,Vector2 direction,string playerTag,float offset=0,Transform target=null,float delay=0)
    {
        if (weaponBase == null) return;
        if (_transform == null)
        {
            _transform = transform;
            defaultScale = _transform.localScale;
        }
        _transform.localScale = defaultScale * Size;
        this.currentBounce = 0;
        this.currentPiercing = 0;
        this.currentRicochet = 0;
        firstImpact = false;
        canRicochet = false;


        mask = weaponBase.mask;
        DmgStat=new Stat(weaponBase.GetDamage());

        this.weaponBase = weaponBase;
        _transform.right = direction;
        _transform.position = (Vector3)triggerPos+_transform.up*offset;


        if (modifierSources == null)
        {
            defaultTrackingLevel = TrackingBulletLevel;
            defaultMaxBounce = MaxBounce;
            defaultMaxPiercing = MaxPiercing;
            defaultMaxRicochet = MaxRicochet;
            modifierSources = new List<ModifierSource>();
            modifierSources.Add(new ModifierSource(Velocity));
            modifierSources.Add(new ModifierSource(MaxPiercing));
            modifierSources.Add(new ModifierSource(MaxBounce));
            modifierSources.Add(new ModifierSource(TrackingBulletLevel));
            modifierSources.Add(new ModifierSource(MaxRicochet));
        }
        foreach(var modifier in modifierSources)
        {
            modifier.Stat.ClearModifiers();
        }
        modifierSources[0].Value = Velocity;
        modifierSources[1].Value = defaultMaxPiercing;
        modifierSources[2].Value = defaultMaxBounce;
        modifierSources[3].Value = defaultTrackingLevel;
        modifierSources[4].Value = defaultMaxRicochet;
        Messenger.Broadcast<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, (ActorBase)weaponBase.character, this, modifierSources);

        Velocity = modifierSources[0].Value==0?Velocity: modifierSources[0].Value;
        MaxPiercing = (int)modifierSources[1].Value==0?defaultMaxPiercing: (int)modifierSources[1].Value;
        MaxBounce = (int)modifierSources[2].Value==0?defaultMaxBounce: (int)modifierSources[2].Value;
        TrackingBulletLevel = modifierSources[3].Value==0? defaultTrackingLevel : modifierSources[3].Value;
        MaxRicochet = (int)modifierSources[4].Value == 0 ? defaultMaxRicochet : (int)modifierSources[4].Value;

        canRicochet = MaxRicochet > 0;
        gameObject.SetActive(true);


        Move().Forget();
        foreach(var impactHandler in ImpactHandlers)
        {
            impactHandler.SetUp(this);
        }

        async UniTask Move()
        {
            await UniTask.Delay((int)(delay * 1000));
            this.MoveHandler.Move(new Stat(Velocity), direction);
            this.MoveHandler.TrackTarget(TrackingBulletLevel, target);
        }
    }


    public virtual void OnVisible(bool isVisible)
    {
        if (!isVisible)
        {
            gameObject.SetActive(false);
        }
    }
    //public float GetBaseDamage()
    //{
    //    return gunBaseDamage;
    //}
    public Vector3 GetDamagePosition()
    {
        return GetTransform().position;
    }
    public Transform GetTransform()
    {
        return _transform;
    }
}   
