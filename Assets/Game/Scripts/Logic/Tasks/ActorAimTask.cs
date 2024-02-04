using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorAimTask : SkillTask
{
    public string aimLineId = "AimLine";
    public LayerMask AimMask;
    [SerializeField] private ValueConfigSearch aimTime,TotalTime ;
    private float timeCtr = 0;
    AimLine aimLiner;

    private float time,totalTime;

    public override async UniTask Begin()
    {
        timeCtr = 0;
        time = aimTime.SetId(Caster.gameObject.name).FloatValue;
        totalTime = TotalTime.SetId(Caster.gameObject.name).FloatValue;
        if (totalTime == 0)
        {
            totalTime = time;
        }
        aimLiner=(await Game.Pool.GameObjectSpawner.Instance.GetAsync(aimLineId)).GetComponent<AimLine>();
        aimLiner.SetUp(Caster.GetTransform(), Vector2.zero, 0);
        await base.Begin();
    } 
    public override void OnStop()
    {
        base.OnStop();
        if (aimLiner != null)
        {
            aimLiner.Hide();
        }
    }
    RaycastHit2D[] hits=new RaycastHit2D[3];
    RaycastHit2D result;
    public override void Run()
    {
        if (IsCompleted || !IsRunning || aimLiner==null) return;
        base.Run();
        timeCtr += Time.deltaTime;
        if (timeCtr > totalTime)
        {
            aimLiner.Hide();
            IsCompleted = true;
        }
        else
        {
            if (Caster.WeaponHandler.CurrentWeapon == null)
            {
                IsCompleted = true;
                aimLiner?.Hide();
                return;
            }
            if (timeCtr<time &&aimLiner!=null &&Caster.Sensor.CurrentTarget != null && Caster.AttackHandler.targetType.Contains(Caster.Sensor.CurrentTarget.GetCharacterType()))
            {
                AimAtTarget(Caster.Sensor.CurrentTarget.GetMidTransform().position, (Character)Caster);

                bool check = false;
                int count =Physics2D.RaycastNonAlloc(Caster.WeaponHandler.GetAttackPoint(Caster.WeaponHandler.CurrentWeapon).position, Caster.GetLookDirection(), hits,9999,layerMask:AimMask);
                for (int i = 0; i < count; i++)
                {

                    ITarget target = hits[i].collider.GetComponentInParent<ITarget>();
                    if (target == null || (target != null && Caster.AttackHandler.targetType.Contains(target.GetCharacterType())))
                    {
                        result = hits[i];
                        check = true;
                        break;
                    }
                }



                aimLiner.SetUp(Caster.WeaponHandler.GetAttackPoint(Caster.WeaponHandler.CurrentWeapon), 
                    Caster.GetLookDirection(),
                    !check ? 99 : result.distance);

            }
            else if (timeCtr >= time && aimLiner != null && Caster.Sensor.CurrentTarget != null && Caster.AttackHandler.targetType.Contains(Caster.Sensor.CurrentTarget.GetCharacterType()))
            {

                bool check = false;
                int count = Physics2D.RaycastNonAlloc(Caster.WeaponHandler.GetAttackPoint(Caster.WeaponHandler.CurrentWeapon).position, Caster.GetLookDirection(), hits, 9999, layerMask: AimMask);
                for (int i=0;i<count;i++)
                {

                    ITarget target = hits[i].collider.GetComponentInParent<ITarget>();
                    if ( target == null || (target != null && Caster.AttackHandler.targetType.Contains(target.GetCharacterType())))
                    {
                        result = hits[i];
                        check = true;
                        break;
                    }
                }

                aimLiner.SetUp(Caster.WeaponHandler.GetAttackPoint(Caster.WeaponHandler.CurrentWeapon),
                    Caster.GetLookDirection(),
                    !check?99:result.distance);
            }
        }
    }

    void AimAtTarget(Vector3 targetPosition,Character character)
    {
        Vector3 direction = (targetPosition - character.GetMidTransform().position).normalized;
        character.SetLookDirection(0, 0);
        character.SetFacing(direction.x > 0 ? 1 : -1);
        character.SetLookDirection(direction.x, direction.y);
        //Debug.DrawLine(character.GetMidTransform().position, targetPosition, Color.yellow, 0.1f);

    }

}