using DG.Tweening;
using Game.GameActor;
using UnityEngine;

[RequireComponent(typeof(InvokeHitByTrigger))]
public class Boss20Skill2BourgeonObject : BulletSimpleDamageObject
{
    public float _height = 3f;
    [SerializeField] private DamageExplode _damageExplode;
    private Stat speedStat;
    private Stat radiusStat;
    private AnimationCurve curve;

    public Stat SpeedStat { get => speedStat; set => speedStat = value; }
    public Stat RadiusStat { get => radiusStat; set => radiusStat = value; }

    private Vector3 targetPos;
    public void Setup(Vector3 targetPos)
    {
        this.targetPos = targetPos;
        _hit.SetIsFullTimeHit(false);
        _hit.SetMaxHit(1);
        _hit.onTrigger += OnTrigger;
        _hit.SetTimeWaitForBegin(1f);

        //rigidbody2D.velocity = calcBallisticVelocityVector(Caster.GetPosition(), targetPos, 45);
        //_move.Move(SpeedStat, targetPos);
    }

    public void SetCurve(AnimationCurve curve)
    {
        this.curve = curve;
    }

    public override void Play()
    {
        base.Play();
        transform.DOJump(targetPos, transform.position.y + _height, 1, 1).SetEase(curve);

        //rigidbody2D.velocity = calcBallisticVelocityVector(Caster.GetPosition(), targetPos, 45);
        //_move.Move(SpeedStat, targetPos);
    }

    Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    private void OnTrigger(Collider2D collider,ITarget target)
    {
        if (target != null)
        {
            if (target.GetCharacterType() == Caster.GetCharacterType()) return;
        }

        var explode = PoolManager.Instance.Spawn(_damageExplode);
        explode.transform.position = transform.position;
        explode.Init(Caster);
        explode.SetDmg(DmgStat.Value);
        explode.SetSize(RadiusStat.Value);
        explode.Explode();
    }
}