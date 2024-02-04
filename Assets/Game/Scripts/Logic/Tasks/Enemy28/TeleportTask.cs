using Cysharp.Threading.Tasks;
using Game.Handler;
using UnityEngine;

public class TeleportTask : SkillTask
{
    public GameObject portalPrefab;
    public ValueConfigSearch delayToTeleport;

    private Vector2 targetPos;
    private GameObject outPortal;
    private float timeCtr = 0;
    public override async UniTask Begin()
    {
        await base.Begin();
        timeCtr = 0;
        targetPos = Caster.Machine.GetState<ActorRandomFlyInAreaItselfState>().GetNewPosition();
        //outPortal = PoolManager.Instance.Spawn(portalPrefab);
        //outPortal.transform.position = targetPos;
        if (Caster.GetCharacterType() != Game.GameActor.ECharacterType.Boss)
        {
            if (HealthBarHandler.Instance.Get(Caster) != null)
                HealthBarHandler.Instance.Get(Caster).SetActive(false);
        }



        Caster.PropertyHandler.AddProperty(Game.GameActor.EActorProperty.Trackable, 0);
        Caster.PropertyHandler.AddProperty(Game.GameActor.EActorProperty.Vunerable, 0);
    }

    public override void OnStop()
    {
        if (outPortal)
            PoolManager.Instance.Despawn(outPortal);
        outPortal = null;
        base.OnStop();
    }

    public override void Run()
    {
        if (IsCompleted) return;
        base.Run();
        timeCtr += Time.deltaTime;
        if (timeCtr > delayToTeleport.SetId(Caster.gameObject.name).FloatValue)
        {
            //if (outPortal)
            //    PoolManager.Instance.Despawn(outPortal);
            outPortal = null;
            Caster.Teleport(targetPos);
            IsCompleted = true;
        Caster.PropertyHandler.AddProperty(Game.GameActor.EActorProperty.Trackable, 1);
            Caster.PropertyHandler.AddProperty(Game.GameActor.EActorProperty.Vunerable, 1);
            HealthBarHandler.Instance.Get(Caster).SetActive(true);


        }
    }
}