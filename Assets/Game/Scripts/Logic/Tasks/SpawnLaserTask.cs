using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SpawnLaserTask : SkillTask
{
    List<LaserEye> laserBeams = new List<LaserEye>();
    public AssetReferenceGameObject laserBeamRef;

    public ValueConfigSearch TotalLaser,Duration,DamageMultiplier, ReadyDuration,LaserSize,Delay;
    public float Offset;

    int emptyBeamIndex;
    public override async UniTask Begin()
    {
        isFired = false;
        isAimLineHidden = false;
        laserBeams.Clear();
        for(int i = 0; i < TotalLaser.IntValue; i++)
        {
            var obj =await Game.Pool.GameObjectSpawner.Instance.GetAsync(laserBeamRef.RuntimeKey.ToString(), Game.Pool.EPool.Projectile);
            LaserEye laserBeam = obj.GetComponent<LaserEye>();
            laserBeams.Add(laserBeam);
            laserBeam.transform.position = Caster.GetPosition() + new Vector3(-3, 1.5f+i*Offset);
            laserBeam.SetActive(true);
        }
        time = Duration.FloatValue;
        readyTime = ReadyDuration.FloatValue;
        delayTime = Delay.FloatValue;
        emptyBeamIndex = UnityEngine.Random.Range(0, TotalLaser.IntValue-1);

        for (int i = 0; i < laserBeams.Count; i++)
        {
            if (i == emptyBeamIndex) continue;
            //Logger.Log(Caster.Stats.GetStat(StatKey.Dmg).Value +" " + DamageMultiplier.FloatValue+" = "+ Caster.Stats.GetStat(StatKey.Dmg).Value * DamageMultiplier.FloatValue);
            var hit = Physics2D.Raycast(laserBeams[i].transform.position, Vector3.left, 99, LayerMask.GetMask("Ground"));

            laserBeams[i].SetUp(Caster,LaserSize.FloatValue,Caster.Stats.GetStat(StatKey.Dmg).Value*DamageMultiplier.FloatValue, Vector3.left, hit.distance);
        }
        await base.Begin();

        for (int i = 0; i < laserBeams.Count; i++)
        {
            if (i == emptyBeamIndex) continue;
            laserBeams[i].ReadyLaser();
        }


    }

    float time = 0,readyTime=0,delayTime=0;
    bool isFired = false,isAimLineHidden=false;
    public override void Run()
    {
        base.Run();
        if (IsCompleted || !IsRunning) return;

        // laser prepare to shoot
        if (readyTime > 0)
        {
            readyTime -= Time.deltaTime;
        }
        else
        {
            // laser is ready, delay a bit. Turn off aim line
            if (delayTime > 0)
            {
                delayTime -= Time.deltaTime;
                if (!isAimLineHidden)
                {
                    isAimLineHidden = true;
                    for (int i = 0; i < laserBeams.Count; i++)
                    {
                        if (i == emptyBeamIndex) continue;
                        laserBeams[i].HideAimLine();
                    }
                }


            }
            //fire laser
            else
            {
                if (!isFired)
                {
                    isFired = true;
                    for (int i = 0; i < laserBeams.Count; i++)
                    {
                        if (i == emptyBeamIndex) continue;
                        laserBeams[i].Fire();
                    }
                }

                //timer for laser duration
                if (time > 0)
                {
                    time -= Time.deltaTime;
                }
                else
                {
                    //turn off laser
                    for (int i = 0; i < laserBeams.Count; i++)
                    {
                        if (i == emptyBeamIndex) continue;
                        laserBeams[i].Release();
                    }
                    IsCompleted = true;
                }
            }
        }
    }

    public override async UniTask End()
    {
        await base.End();
        await UniTask.Delay(500);
        foreach (var laserBeam in laserBeams)
        {
            laserBeam.SetActive(false);
        }
        laserBeams.Clear();
        isFired = false;

    }
}
