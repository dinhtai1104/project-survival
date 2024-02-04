using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;
using UnityEngine;

public class MoveTowardTargetAction : Action
{
    ActorBase actor;
    [UnityEngine.SerializeField]
    private ValueConfigSearch MoveSpeedMultiplier ,ChaseDuration    ;
    public float chaseTimer = 0;

    public SharedBool isChasing;
    public SharedTransform target;
    Vector3 position;
    LayerMask groundMask;
    private float moveSpeedMultiplier, chaseDuration;
    [SerializeField]
    float checkColliderWithObjectTime = 0.2f;
    public float highSpeedSwitchTime = 1f;
    public override void OnAwake()
    {
        base.OnAwake();
        groundMask = LayerMask.GetMask("Ground");
        actor = GetComponent<ActorBase>();
        chaseDuration = ChaseDuration.SetId(actor.gameObject.name).FloatValue;
        moveSpeedMultiplier = MoveSpeedMultiplier.SetId(actor.gameObject.name).FloatValue;

    }
    public override void OnStart()
    {
        base.OnStart();
        chaseTimer = 0;
        isChasing.Value = true;
        isStarted = false;
        actor.AnimationHandler.GetAnimator().AnimationState.SetAnimation(0, "attack/combo_1_1", false).Complete += MoveTowardTargetAction_Complete; ;
        startTimer = Time.time;
        position = target.Value.position;
        timer = 0;
    }

    private void MoveTowardTargetAction_Complete(Spine.TrackEntry trackEntry)
    {
        actor.AnimationHandler.GetAnimator().AnimationState.SetAnimation(0, "attack/combo_1_2", true);
        isStarted = true;

    }
    [SerializeField]
    bool isStarted = false;
    [SerializeField]
    float timer = 0;
    [SerializeField]
    float startTimer = 0;
    [SerializeField]
    bool isHighSpeedChase = false;
    public override TaskStatus OnUpdate()
    {
        if (!isStarted) return TaskStatus.Running;
        actor.MoveHandler.Move((position - actor.GetPosition()).normalized, moveSpeedMultiplier);
        chaseTimer += Time.deltaTime;


        if (Time.time - timer >= checkColliderWithObjectTime && (Vector2.Distance(actor.GetPosition(), position) < 1f || chaseTimer>2.5f ||CheckCollideWithObject(actor.GetMidTransform().position)))
        {
            timer = Time.time;
            isChasing.Value = false;
            actor.AnimationHandler.GetAnimator().AnimationState.SetAnimation(0, "attack/combo_1_3", false);
            actor.AnimationHandler.GetAnimator().AnimationState.AddAnimation(0, "move", true,0);

            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
    public Collider2D[] colliders = new Collider2D[1];
    private bool CheckCollideWithObject(Vector2 position)
    {
        return false;
        if (Physics2D.OverlapCircleNonAlloc(position, 1, colliders,groundMask) >0)
        {
            return true;
        }
        colliders[0] = null;
        return false;

    }
}