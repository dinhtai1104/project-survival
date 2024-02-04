using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Enemy7003_Skill2_Task : SkillTask
{

    public ValueConfigSearch totalBullet;
    public ValueConfigSearch fireRate;

    public Transform tail,shootPoint;

    public int[] offsetAngles;

    [SerializeField]
    private string anim;


    public WeaponBase weapon;
    public WeaponBase weaponInstance;


    public string VFX_Shot = "VFX_Boss_3002_Shoot";

    public override async UniTask Begin()
    {
        await base.Begin();
        if (weaponInstance == null)
        {
            weaponInstance = await weapon.SetUp((Game.GameActor.Character)Caster);
        }
        if (Caster.FindClosetTarget() == null)
        {
            IsCompleted = true;
            return;
        }
        fireRate.SetId(Caster.gameObject.name);
        totalBullet.SetId(Caster.gameObject.name);

        ReleaseBullet();

    }

    public override async UniTask End()
    {
        await base.End();
       
    }
    public override void OnStop()
    {
        base.OnStop();
    }


    bool isTriggered = false;
    private async UniTask ReleaseBullet()
    {
        var target = Caster.Sensor.CurrentTarget;
        if (target == null)
        {

            return;
        }
        isTriggered = true;

        //direction = (target.GetMidTransform().position - tail.position).normalized;

        await UniTask.Delay(500);
        for (int i = 0; i < totalBullet.IntValue; i++)
        {

            //direction = (target.GetMidTransform().position - tail.position).normalized;

            ReleaseBullet(shootPoint, target, /*Quaternion.AngleAxis(offsetAngles.Random(), Vector3.forward) **/ (target.GetMidTransform().position - shootPoint.position).normalized);
            await UniTask.Delay((int)(1000 / fireRate.FloatValue));
        }
        //await UniTask.Delay(1000);
        //isTriggered = false;
        //float time = 0.5f;
        //direction = new Vector3(-1, 0);
        //while (time > 0)
        //{
        //    time -= Time.deltaTime;
        //    tail.localEulerAngles = Vector3.Lerp(tail.localEulerAngles, Vector3.zero, 0.15f);
        //    await UniTask.Yield();
        //}
        IsCompleted = true;
      
    }
 
    void ReleaseBullet(Transform dropPoint,ITarget target,Vector3 direction)
    {
        Caster.AnimationHandler.SetAnimation(1, anim, false);
        Caster.AnimationHandler.AddEmptyAnimation(1);
        weaponInstance.Trigger(dropPoint, target.GetMidTransform(), direction, target,null);

    }
     Vector3 direction=new Vector3(-1,0);

    public override void Run()
    {
        base.Run();
        //if (isTriggered)
        //{

        //    tail.right = Vector3.Lerp(tail.right, direction, 0.1f);
        //}
    }




}