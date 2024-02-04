using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HealthBarBase : MonoBehaviour
{
    protected ActorBase actor;

    [SerializeField]
    protected TMPro.TextMeshProUGUI healthText;
    Vector3 offset;
    Transform _transform;
    bool active = false;
    public void SetUp(ActorBase actor,Vector2 offset)
    {
        _transform = transform;
        this.actor = actor;
     
        this.offset = offset;
        _transform.localPosition = actor.GetPosition() + this.offset;
        active = true;
        //SetActive(true);
        Init();
        actor.HealthHandler.onUpdate += OnUpdate;
        actor.HealthHandler.onHealthDepleted += OnHealthDepleted;
        actor.HealthHandler.onArmorBroke += OnArmorBroke;
        OnUpdate(actor.HealthHandler);

    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    private void OnEnable()
    {
        //if (actor == null) return;
        //Logger.Log("OnEnable " + gameObject.name+" " +actor.gameObject.name);
       
        //OnUpdate(actor.HealthHandler);

    }
    private void OnDisable()
    {
    }
    private void OnDestroy()
    {
   
    }
    protected virtual void Update()
    {
        if (!active) return;
        _transform.localPosition = actor.GetPosition() + offset;
    }
    protected abstract void Init();
    protected abstract void OnUpdate(HealthHandler health);
    protected abstract void OnHealthDepleted();
    protected abstract void OnArmorBroke();

    public virtual void Hide()
    {
        Clear();
        SetActive(false);
       

    }
    void Clear()
    {
        Logger.Log("CLEAR HEALTHBAR: "+gameObject.name+" => " + active+" : "+(actor == null));
        active = false;
        if (actor == null) return;
        actor.HealthHandler.onUpdate -= OnUpdate;
        actor.HealthHandler.onHealthDepleted -= OnHealthDepleted;
        actor.HealthHandler.onArmorBroke -= OnArmorBroke;
        actor = null;
    
    }
}
