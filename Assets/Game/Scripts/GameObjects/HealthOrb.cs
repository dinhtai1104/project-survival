using Game.GameActor;
using MoreMountains.Feedbacks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthOrb : MonoBehaviour, ICollectable
{
    BelzierMoveHandler moveHandler;

    public ECharacterType targetType;

    private ActorBase target;
    private ActorBase Target
    {
        get
        {
            if (target == null)
            {
                var actors = GameObject.FindObjectsOfType<ActorBase>();
                foreach (var actor in actors)
                {
                    if (actor.GetCharacterType() == targetType)
                    {
                        target = actor;
                        break;
                    }
                }
            }
            return target;
        }
    }

    [SerializeField]
    private MMF_Player startFb;
    public void SetUp(float healRate, Vector3 position)
    {
        moveHandler = GetComponent<BelzierMoveHandler>();
        transform.position = position;

        SetActive(true);

        moveHandler.Move(Target.GetMidTransform(), ()=> 
        {
            Target.Heal(healRate * Target.HealthHandler.GetMaxHP());

            SetActive(false);
        });

    }

  

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Collect(Collector collector)
    {
    }

    public void MoveToward(Collector collector)
    {
    }
}
