using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Enemy7003_Skill1_Task : SkillTask
{

    public ValueConfigSearch randomMin,randomMax;


    public WeaponBase weapon;
    public WeaponBase weaponInstance;

    [SerializeField]
    private Transform[] dropPoints;

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
        randomMax.SetId(Caster.gameObject.name);
        randomMin.SetId(Caster.gameObject.name);
        ReleaseBullet();

    }

    public override UniTask End()
    {
        return base.End();
    }
    public override void OnStop()
    {
        base.OnStop();
    }


    List<Transform> points = new List<Transform>();

    private async UniTask ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        points.Clear();
        int totalBomb = UnityEngine.Random.Range(randomMin.IntValue, randomMax.IntValue + 1);
        List<Transform> tempPoints = new List<Transform>(this.dropPoints);

        for (int i = 0; i < totalBomb; i++)
        {
            Transform dropPoint = tempPoints.Random();

            tempPoints.Remove(dropPoint);

            points.Add(dropPoint);
        }
        await UniTask.Delay(500);

        foreach (var point in points)
        {
             ReleaseBullet(point,target);
        }
        IsCompleted = true;
      
    }
    int GetIndex(Transform point)
    {
        for(int i = 0; i < dropPoints.Length; i++)
        {
            if (point == dropPoints[i])
            {
                return i;
            }
        }
        return 0;
    }
    async UniTask ReleaseBullet(Transform dropPoint,ITarget target)
    {
        //Game.Pool.GameObjectSpawner.Instance.GetAsync(VFX_Shot).ContinueWith(obj =>
        //{
        //    obj.GetComponent<Game.Effect.EffectAbstract>().Active(dropPoint.position).SetParent(dropPoint);
        //}).Forget();
        Caster.AnimationHandler.SetAnimation(GetIndex(dropPoint)+1, "combo/combo_1_" + (GetIndex(dropPoint) + 1), false);
        Caster.AnimationHandler.AddEmptyAnimation(GetIndex(dropPoint)+1);
        weaponInstance.Trigger(dropPoint, target.GetMidTransform(), Vector2.down, target,null);

    }

 


}