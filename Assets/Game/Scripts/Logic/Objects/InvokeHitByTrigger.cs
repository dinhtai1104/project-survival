using com.mec;
using Game.GameActor;
using Game.Pool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class InvokeHitByTrigger : MonoBehaviour
{
    public bool Poolable = true;
    public bool SpawnEff = true;
    public string VFX_IMPACT = "VFX_BulletImpact";
    public delegate void OnTrigger(Collider2D collider,ITarget target);
    public OnTrigger onTrigger;
    public OnTrigger onTriggerWhenCollideAnything;

    public CharacterObjectBase Base;

    private List<IHitTriggerAction> hitTriggerActions= new List<IHitTriggerAction>();
    private List<IBeforeDestroyObject> _beforeDestroyObject = new List<IBeforeDestroyObject>();
    private bool isInit = false;


    private bool isFulltimeHit = false;
    private int maxHit = 0;
    private int currentHit = 0;
    [SerializeField] private LayerMask layerMaskTarget;
    [SerializeField] private bool isUseIgnoreMask;
    [SerializeField, ShowIf(nameof(isUseIgnoreMask), true)] private LayerMask ignoreLayerMask;
    [SerializeField] private float _timeInterval = 0;
    [SerializeField] private float _timeWaitBegin = 0;
    private bool _isCooling = false;


    private void OnEnable()
    {
        if (isInit == false)
        {
            isInit = true;
            hitTriggerActions = new List<IHitTriggerAction>(GetComponents<IHitTriggerAction>());
            _beforeDestroyObject = new List<IBeforeDestroyObject>(GetComponents<IBeforeDestroyObject>());
            Base = GetComponent<CharacterObjectBase>();
        }
        lastCheck = 0;
        isFirstTrigger = false;
        _isCooling = false;
        currentHit = 0;
        if (_timeWaitBegin > 0)
        {
            Timing.RunCoroutine(_Cooling(_timeWaitBegin));
        }
    }

    public void SetIntervalTime(float time)
    {
        _timeInterval = time;
    }

    public void SetTimeWaitForBegin(float time)
    {
        this._timeWaitBegin = time;
        if (time > 0)
        {
            _isCooling = true;
        }
        Timing.RunCoroutine(_Cooling(time));
    }
    public void SetIsFullTimeHit(bool isFulltimeHit)
    {
        this.isFulltimeHit = isFulltimeHit;
    }

    public void SetMaxHit(int maxHit)
    {
        this.maxHit = maxHit;
    }

    protected bool HasLayer(int layer)
    {
        return ((1 << layer) & layerMaskTarget) != 0;
    }
    private bool isFirstTrigger = false;
    float lastCheck = 0;
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (_isCooling || isFirstTrigger == false) return;
        if (Time.time - lastCheck < _timeInterval) return;
        //if (!HasLayer(collision.gameObject.layer))
        //{
        //    return;
        //}
        ITarget target = collision.GetComponentInParent<ITarget>();
        if (target == null) return;
        Impact(collision,target);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isCooling) return;
        //if (!HasLayer(collision.gameObject.layer))
        //{
        //    return;
        //}
        isFirstTrigger = true;
        ITarget target = collision.GetComponentInParent<ITarget>();
        
        onTriggerWhenCollideAnything?.Invoke(collision,target);
        Impact(collision,target);
    }

    private void Impact(Collider2D collision,ITarget target)
    {
        //if (Time.time - lastCheck < _timeInterval) return;
        if (Base.Caster == null)
        {
            Base.gameObject.SetActive(false);
        }
        if (target == null || (target != null && (UnityEngine.Object)target != Base.Caster && Base.Caster.AttackHandler.IsValid(target.GetCharacterType())))
        {
            if (target != null)
            {
                if (isUseIgnoreMask && !HasLayer(ignoreLayerMask)) return;
                //Logger.Log("=.......> ipmact " + target.GetTransform().name + " " + target.ToString() + " " + (onTrigger == null)+ " "+ gameObject.name);
                _isCooling = true;
            }

            lastCheck = Time.time;
            onTrigger?.Invoke(collision, target);
            currentHit++;

            var pos = transform.position;
            var vfx_impact = VFX_IMPACT;

            foreach (var hitAction in hitTriggerActions)
            {
                hitAction.Action(collision);
            }

            if (SpawnEff)
            {

                // Spawn Impact Effect
                GameObjectSpawner.Instance.Get(vfx_impact, res =>
                {
                    res.GetComponent<Game.Effect.EffectAbstract>().Active(pos);
                });
            }
            if (!isFulltimeHit)
            {
                if (currentHit >= maxHit)
                {
                    onTrigger = null;
                    if (gameObject.activeSelf)
                    {
                        foreach (var hitAction in _beforeDestroyObject)
                        {
                            hitAction.Action(collision);
                        }
                        if (Poolable)
                        {
                            PoolManager.Instance.Despawn(gameObject);
                        }
                        else
                        {
                            gameObject.SetActive(false);
                        }
                    }
                    return;
                }
            }
            Timing.RunCoroutine(_Cooling(_timeInterval));
        }
    }
    private IEnumerator<float> _Cooling(float time)
    {
        _isCooling = true;
        yield return Timing.WaitForSeconds(time);
        _isCooling = false;
    }
}