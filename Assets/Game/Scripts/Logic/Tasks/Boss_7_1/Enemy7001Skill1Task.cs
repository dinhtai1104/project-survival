using com.mec;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class Enemy7001Skill1Task : SkillTask
{
    public string _animation = "attack/combo_1";
    public string _animationLoop = "attack/combo_1";

    public float heightoffset = 3f;

    public GameObject predictBigStonePrefab;
    public BulletSimpleDamageObject bigStonePrefab;
    public BulletSimpleDamageObject smallStonePrefab;
    [Header("Feedbacks")]
    public MMF_Player feedback_ShakeCam;

    [Header("Big Stone")]
    public ValueConfigSearch bigStone_Number;
    public ValueConfigSearch bigStone_Dmg;
    public ValueConfigSearch bigStone_Size;
    public ValueConfigSearch bigStone_DelayBtwStone;
    public ValueConfigSearch bigStone_DelayStart;
    public ValueConfigSearch bigStone_Gravity;

    [Header("Small Stone")]
    public ValueConfigSearch smallStone_Number;
    public ValueConfigSearch smallStone_Dmg;
    public ValueConfigSearch smallStone_Size;
    public ValueConfigSearch smallStone_DelayBtwStone;
    public ValueConfigSearch smallStone_DelayStart;
    public ValueConfigSearch smallStone_Gravity;

    private CoroutineHandle _bigStoneHandle, _smallStoneHandle;

    int smallStoneLeft, bigStoneleft;

    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.AnimationHandler.SetAnimation(_animation, false);
        Caster.AnimationHandler.AddAnimation(0, _animationLoop, true);

        smallStoneLeft = smallStone_Number.IntValue;
        bigStoneleft = bigStone_Number.IntValue;

        _bigStoneHandle = Timing.RunCoroutine(_BigStoneFall(), gameObject);
        _smallStoneHandle = Timing.RunCoroutine(_SmallStoneFall(), gameObject);
        Timing.RunCoroutine(_DelayPlayFeedbacks(), gameObject);
        Timing.RunCoroutine(_WaitForComplete(), gameObject);
    }

    private IEnumerator<float> _WaitForComplete()
    {
        while (smallStoneLeft > 0 || bigStoneleft > 0)
        {
            yield return Timing.DeltaTime;
        }
        IsCompleted = true;
    }

    private IEnumerator<float> _DelayPlayFeedbacks()
    {
        yield return Timing.WaitForSeconds(0.5f);
        feedback_ShakeCam.PlayFeedbacks();
    }
    public override void OnStop()
    {
        base.OnStop();
        feedback_ShakeCam.StopFeedbacks();
    }
    public override UniTask End()
    {
        feedback_ShakeCam.StopFeedbacks();
        return base.End();
    }

    private IEnumerator<float> _BigStoneFall()
    {
        yield return Timing.WaitForSeconds(bigStone_DelayStart.FloatValue);
        for (int i = 0; i < bigStone_Number.IntValue; i++)
        {
            StoneFalling();
            yield return Timing.WaitForSeconds(bigStone_DelayBtwStone.FloatValue);
            bigStoneleft--;
        }

        void StoneFalling()
        {
            var target = Caster.FindClosetTarget();
            if (target == null) { return; }

            var heightScreen = Camera.main.orthographicSize * 2 / Camera.main.aspect;

            var stone = PoolManager.Instance.Spawn(bigStonePrefab);
            stone.transform.position = new Vector3(target.GetMidPos().x, heightScreen + heightoffset);
            stone.SetSize(bigStone_Size.FloatValue);
            stone.SetCaster(Caster);
            stone.GetComponent<Rigidbody2D>().gravityScale = bigStone_Gravity.FloatValue;
            stone.DmgStat = new Stat(Caster.GetStatValue(StatKey.Dmg) * bigStone_Dmg.FloatValue);
            stone.Movement.SetDirection(Vector3.down);
            stone.Play();
        }
    }

    private IEnumerator<float> _SmallStoneFall()
    {
        yield return Timing.WaitForSeconds(smallStone_DelayStart.FloatValue);

        for (int i = 0; i < smallStone_Number.IntValue; i++)
        {
            StoneFalling();
            yield return Timing.WaitForSeconds(smallStone_DelayBtwStone.FloatValue);
            smallStoneLeft--;
        }

        void StoneFalling()
        {
            var widthScreen = Camera.main.orthographicSize;
            var heightScreen = Camera.main.orthographicSize * 2 / Camera.main.aspect;

            var xPos = UnityEngine.Random.Range(-widthScreen, widthScreen);

            var stone = PoolManager.Instance.Spawn(smallStonePrefab);
            stone.transform.position = new Vector3(xPos, heightScreen + heightoffset);
            stone.SetSize(smallStone_Size.FloatValue);
            stone.SetCaster(Caster);
            stone.DmgStat = new Stat(Caster.GetStatValue(StatKey.Dmg) * smallStone_Dmg.FloatValue);
            stone.Movement.SetDirection(Vector3.down);

            stone.GetComponent<Rigidbody2D>().gravityScale = smallStone_Gravity.FloatValue;
            stone.Play();
        }
    }
}