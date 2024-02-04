using Game.Effect;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatedSawBullet : BulletBase
{
   
    bool isSawActivated = false;
    public ValueConfigSearch IdleAttackDuration;
    [SerializeField]
    private float hitSpeed=0.2f,returnSpeed=12f,idleAttackDuration;
    [SerializeField]
    private Vector3 idleRot,moveRot;
    Vector3 destination;
    Vector2 triggerPos;

    [SerializeField]
    public MoreMountains.Feedbacks.MMF_Player startFb, impactFb,returnFb;
    public override void SetUp(WeaponBase weaponBase, Vector2 triggerPos, Vector2 direction, string playerTag, float offset = 0, Transform target = null, float delay = 0)
    {
        this.triggerPos = triggerPos;
        base.SetUp(weaponBase, triggerPos, direction, playerTag, offset,target,delay);

        returnSpeed = Velocity;
        isSawActivated = false;
        destination = target.position;
        idleAttackDuration = IdleAttackDuration.SetId(weaponBase.character.gameObject.name).FloatValue;
        startFb?.PlayFeedbacks();
    }


    public void ActiveSaw()
    {
        startFb?.StopFeedbacks();
        impactFb?.PlayFeedbacks();

        isSawActivated = true;
        time = 0;
        InvokeRepeating(nameof(Impact), 0, hitSpeed);
    }

    void Impact()
    {
        foreach (var impactHandler in ImpactHandlers)
        {
            impactHandler.Impact(null);
        }
    }
    float time = 0;
    protected void FixedUpdate()
    {
        if (isSawActivated)
        {
            _transform.Rotate(idleRot);
            if (time < idleAttackDuration)
            {
                _transform.Rotate(idleRot);
                time += Time.fixedDeltaTime;
            }
            else
            {
                // come back to trigger pos
                if (Vector2.Distance(_transform.position, triggerPos) < 0.2f)
                {
                    isSawActivated = false;
                    gameObject.SetActive(false);
                    CancelInvoke();
                    onBulletDeactive?.Invoke(this, weaponBase.character);
                    //
                    impactFb?.StopFeedbacks();
                    returnFb?.PlayFeedbacks();
                }
                else
                {
                    _transform.position = Vector2.MoveTowards(_transform.position, triggerPos, Time.fixedDeltaTime * returnSpeed);
                }
            }
        }
        //object fly toward target
        else
        {
            _transform.Rotate(moveRot);
            //keep moving
            if (Vector2.Distance(_transform.position, destination) > 1f)
            {
                this.MoveHandler.OnUpdate();
            }
            //reach target
            else
            {
                //trigger attack repeatedly
                ActiveSaw();
            }

        }
    }

}
