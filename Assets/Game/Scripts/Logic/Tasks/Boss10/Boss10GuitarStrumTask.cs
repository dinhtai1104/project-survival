using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

public class Boss10GuitarStrumTask : SkillTask
{
    [SerializeField] private string _animationStrumGuitar;
    [SerializeField] private string _animationIdle;
    [SerializeField] private ValueConfigSearch _amountBullet;
    [SerializeField] private ValueConfigSearch _cooldownBtwAttack;
    [SerializeField] private ValueConfigSearch _sizeBullet;
    [SerializeField] private ValueConfigSearch _dmgBullet;
    [SerializeField] private ValueConfigSearch _velocityBullet;
    public EventActionSpine[] _eventActionCb;

    [SerializeField] private Boss10Skill2CircleBalls _circleBallPrefab;
    [SerializeField] private string VFX_GuitarStrum = "VFX_Boss@10_GuitarStrum";
    private ActorBase player;

    private int attackCurrent = 0;
    private bool finishAttack = false;
    private float cooldownTimer = 0;
    public override async UniTask Begin()
    {
        player = GameController.Instance.GetMainActor();
        await base.Begin();
        attackCurrent = 0;
        cooldownTimer = 0;
        finishAttack = false;
        Caster.MoveHandler.ClearBoostVelocity();
        Caster.AnimationHandler.SetAnimation(_animationStrumGuitar, false);
        Caster.AnimationHandler.onEventTracking += OnTriggerEventAnimation;
        Caster.AnimationHandler.onCompleteTracking += OnTriggerComplete;

    }
    public override async UniTask End()
    {
        Caster.AnimationHandler.onEventTracking -= OnTriggerEventAnimation;
        Caster.AnimationHandler.onCompleteTracking -= OnTriggerComplete;
        await base.End();
    }

    private void OnTriggerComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name.Equals(_animationStrumGuitar))
        {
            attackCurrent++;
            finishAttack = true;
            Caster.AnimationHandler.SetAnimation(_animationIdle, true);

            IsCompleted = true;
        }
    }

    private void OnTriggerEventAnimation(Spine.TrackEntry track, Spine.Event e)
    {
        if (!track.Animation.Name.Equals(_animationStrumGuitar)) return;
        for (int i = 0; i < _eventActionCb.Length; i++)
        {
            if (_eventActionCb[i]._eventKey.Equals(e.Data.Name))
            {
                _eventActionCb[i].Action?.Invoke();
            }
        }
    }

    public override void Run()
    {
        if (IsCompleted) { return; }
        base.Run();
        if (!finishAttack) return;

        if (player != null)
        {
            Caster.SetFacing(player);
        }

        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= _cooldownBtwAttack.FloatValue)
        {
            cooldownTimer = 0;
            finishAttack = false;
            Caster.AnimationHandler.SetAnimation(_animationStrumGuitar, false);
        }
    }

    public void SpawnBullet()
    {
        var pos = Caster.GetMidTransform().position;
        // Spawn Eff
        Game.Pool.GameObjectSpawner.Instance.Get(VFX_GuitarStrum, res =>
        {
            res.GetComponent<Game.Effect.EffectAbstract>().Active(pos, Caster.transform.localScale.x);
        });

        Debug.Log("Spawn Bullet: on Boss 10");
        var obj = PoolManager.Instance.Spawn(_circleBallPrefab);
        obj.transform.position = pos;
        var directon = GameController.Instance.GetMainActor().GetPosition() - pos;
        var angle = Mathf.Atan2(directon.y, directon.x) * Mathf.Rad2Deg;
        obj.SetRadius(1f);
        obj.SetAngleStart(angle);
        obj.SetPosition(pos);
        obj.SpawnItem(_amountBullet.IntValue, _sizeBullet.FloatValue, OnSpawnBulet);
    }

    private void OnSpawnBulet(Boss10Skill2BallObject bullet)
    {
        if (bullet != null)
        {
            var dmgEnemy = Caster.Stats.GetValue(StatKey.Dmg) * _dmgBullet.FloatValue;
            var speedBullet = _velocityBullet.FloatValue;
            bullet.SetCaster(Caster);
            bullet.DmgStat = new Stat(dmgEnemy);
            bullet.SpeedStat = new Stat(speedBullet);

            bullet.Play();
        }
    }
}

[System.Serializable]
public class EventActionSpine
{
    public string _eventKey;
    public UnityEvent Action;
}