using UnityEngine;

public class Enemy6001Skill1Object : BulletSimpleDamageObject
{
    private float startAngle;
    private float currentAngle;
    private bool isReleased = false;
    public Sensor sensor;
    private IGetPositionByAngle Position;
    private Stat speedInCharge;

    private void Awake()
    {
        sensor = GetComponent<Sensor>();
    }

    private void OnEnable()
    {
        Animation.SetAnimation("idle", true);
    }

    public override void Play()
    {
        isReleased = false;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Movement.Stop();
    }
    public void SetSpeed(Stat speed, Stat speedInCharge)
    {
        Movement.Speed = speed;
        this.speedInCharge = speedInCharge;
    }

    public void SetAngle(float angle)
    {
        startAngle = currentAngle = angle;
    }

    public void OnUpdate()
    {
        if (isReleased) { return; }
        currentAngle += Time.fixedDeltaTime * speedInCharge.Value;
        var pos = Position.GetPosition(currentAngle);
        transform.position = pos;

        Movement.SetDirection(GameController.Instance.GetMainActor().GetMidTransform().position - transform.position);
    }

    public void Release()
    {
        var target = GameController.Instance.GetMainActor();
        if (target == null) return;
        isReleased = true;

        Movement.SetDirection(target.GetMidTransform().position - transform.position);
        Animation.SetAnimation("attack", true);
        base.Play();
    }

    public void SetPosition(IGetPositionByAngle elipseObject)
    {
        Position = elipseObject;
    }
}