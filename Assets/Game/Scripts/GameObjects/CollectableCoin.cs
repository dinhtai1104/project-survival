using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCoin : MonoBehaviour,ICollectable
{
    [SerializeField]
    private  string CollectEffectId = "VFX_CoinCollect";
    [SerializeField]
    private float StartForce;
    [SerializeField]
    Rigidbody2D rb;

    BelzierMoveHandler moveHandler;

    bool isMoving = false;
    void OnEnable()
    {
        isMoving = false;
        moveHandler = GetComponent<BelzierMoveHandler>();

        Vector2 force = UnityEngine.Random.insideUnitCircle;
        force.y = Mathf.Max(0.3f, Mathf.Abs(force.y));
        rb.AddForce(force * StartForce, ForceMode2D.Impulse);


        if (Game.Controller.Instance.gameController.isStageCleared)
        {
            OnGameClear(false);
        }
        else
        {
            Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
        }
        

    }
    void OnDisable() 
    {
        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);

    }


    private void OnGameClear(bool arg1)
    {
        Invoke(nameof(Move), UnityEngine.Random.Range(0, 0.6f));

       

    }
    void Move()
    {
        MoveToward(Game.Controller.Instance.gameController.GetMainActor().GetComponentInChildren<Collector>());
    }
    public void Collect(Collector collector)
    {

        SetActive(false);
    }

    public void MoveToward(Collector collector)
    {
        if (isMoving) return;
        isMoving = true;
        moveHandler.Move(collector.transform, () =>
        {
            SetActive(false);
        });
    }
    
    public void SetActive(bool active)
    {
        Game.Pool.GameObjectSpawner.Instance.Get(CollectEffectId, obj =>
        {
            obj.GetComponent<Game.Effect.EffectAbstract>().Active(transform.position);
        });
        gameObject.SetActive(active);
    }

    
}

public interface ICollectable
{
    void Collect(Collector collector);
    void MoveToward(Collector collector);
    void SetActive(bool active);
}