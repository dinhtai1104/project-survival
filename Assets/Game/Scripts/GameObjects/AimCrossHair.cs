using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimCrossHair : MonoBehaviour
{
    Transform _transform;
    Animator anim;
    ActorBase actor;
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (this.target != null)
            this.target.HighLight(false);
    }
    DetectTargetHandler actorTargetDetector;
    public void SetUp(ActorBase actor)
    {
        if (this.actor != null)
        {
            this.actor.onActorDie -= OnDie;
            this.actor.GetComponent<DetectTargetHandler>().onTargetFound -= OnTargetFound;
        }
        this.actor = actor;
        anim = GetComponent<Animator>();
        _transform = transform;
        actorTargetDetector=actor.GetComponent<DetectTargetHandler>();
        actor.onActorDie += OnDie;

        actorTargetDetector.onTargetFound -= OnTargetFound;
        actorTargetDetector.onTargetFound += OnTargetFound;

    }
  
  
    private void OnTargetFound(ActorBase character, ITarget target)
    {
        if (target == null)
        {
            SetActive(false);
        }
        else
        {
            SetActive(true);
        }
    }

    private void OnDie()
    {
        SetActive(false);
    }
    private void Update()
    {
        OnAimAtTarget(actorTargetDetector.CurrentTarget);
    }
    bool foundTarget = false;
    ITarget target;
    void OnAimAtTarget( ITarget target)
    {
        if (target == null)
        {
            if (foundTarget)
            {
                foundTarget = false;
                this.target.HighLight(false);
                this.target = null;
            }
            SetActive(false);
        }
        else
        {
            if (!foundTarget)
            {
                SetActive(true);
                foundTarget = true;
            }
            if (target != this.target)
            {
                if (this.target != null)
                    this.target.HighLight(false);
                anim.SetTrigger("FoundTarget");
                target.HighLight(true);
            }
            this.target = target;
            _transform.localPosition = target.GetMidTransform().position;
        }
    }
}
