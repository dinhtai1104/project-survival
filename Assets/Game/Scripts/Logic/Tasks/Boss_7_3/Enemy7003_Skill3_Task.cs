using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Enemy7003_Skill3_Task : SkillTask
{

    public ValueConfigSearch DropPoints;


    public WeaponBase weapon;
    public WeaponBase weaponInstance;

    [SerializeField]
    private Transform dropPoint;
    public float[] dropPoints;

    private void OnEnable()
    {
        string[] splits = DropPoints.SetId(Caster.gameObject.name).StringValue.Split(',');
        dropPoints = new float[splits.Length];
        for(int i = 0; i < dropPoints.Length; i++)
        {
            dropPoints[i] = float.Parse(splits[i]);
        }
    }
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

        WaitForDropPoint().ContinueWith(ReleaseBullet).Forget();

    }

    public override UniTask End()
    {
        return base.End();
    }
    public override void OnStop()
    {
        base.OnStop();
    }


    private async UniTask WaitForDropPoint()
    {
        while (!IsCloseToAnyPoint())
        {
            await UniTask.Yield();
        }

        bool IsCloseToAnyPoint()
        {
            float currentPosition = dropPoint.position.x;
            foreach(var point in dropPoints)
            {
                if (Mathf.Abs(currentPosition - point) < 0.2f)
                {

                    return true;
                }
            }
            return false;
        }
    }
    private async UniTask ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        if (target != null)
        {
            weaponInstance.Trigger(dropPoint, target.GetMidTransform(), Vector2.down, target, null);
        }
        IsCompleted = true;

    }
    

 


}