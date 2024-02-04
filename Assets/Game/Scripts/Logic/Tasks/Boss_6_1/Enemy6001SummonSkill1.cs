using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy6001SummonSkill1 : CharacterObjectBase
{
    public Enemy6001Skill1Object penguinPrefab;
    public ValueConfigSearch penguinSize;
    public ValueConfigSearch penguinVelocity;
    public ValueConfigSearch penguinDmg;
    public ValueConfigSearch penguinReflect;
    public ValueConfigSearch penguinNumber;
    public ValueConfigSearch penguinMoveSpeedInCharge;
    public ValueConfigSearch penguinReleaseEachAngle;

    private List<Enemy6001Skill1Object> listPenguin = new List<Enemy6001Skill1Object>();
    public ElipseObject elipseObject;
    private CancellationTokenSource cts;
    public string VFX_Spawn = "VFX_SpawnEnemy";
    public override void Play()
    {
        base.Play();
        if (cts != null)
        {
            cts.Cancel();
        }
        cts = new CancellationTokenSource();
        angle = 0;
        ReleasePenguin();
    }
    protected void ReleasePenguin()
    {
        Timing.RunCoroutine(_ReleaseDelay(), gameObject);
        
    }
    public override void Stop()
    {
        if (cts != null)
        {
            cts.Cancel();
        }
        cts = null;
        base.Stop();
        Timing.KillCoroutines(gameObject);
        foreach (var p in listPenguin)
        {
            PoolManager.Instance.Despawn(p.gameObject);
        }
        listPenguin.Clear();
    }
    private IEnumerator<float> _ReleaseDelay()
    {
        float angle = 0;
        var angleIncre = 360 / penguinNumber.IntValue;
        for (int i = 0; i < penguinNumber.IntValue; i++)
        {
            _SpawnPenguin(angle);
            angle += angleIncre;
        }
        yield return Timing.WaitForSeconds(1.5f);
        isRotate = true;
    }

    private void _SpawnPenguin(float angle)
    {
        GameObjectSpawner.Instance.GetAsync(VFX_Spawn).ContinueWith(async (t) =>
        {
            var eff = t.GetComponent<Game.Effect.EffectAbstract>();
            eff.transform.position = elipseObject.GetPosition(angle);
            eff.transform.SetParent(elipseObject.transform);

            if (eff != null)
            {
                eff.Active();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: cts.Token);
            var bullet = ReleasePenguin(angle);
            bullet.SetPosition(elipseObject);
            listPenguin.Add(bullet);
        }).AttachExternalCancellation(cts.Token);
    }

    private Enemy6001Skill1Object ReleasePenguin(float angle)
    {
        var penguin = PoolManager.Instance.Spawn(penguinPrefab);
        penguin.transform.position = Caster.GetMidTransform().position;

        penguin.transform.eulerAngles = Vector3.forward * angle;
        penguin.transform.localScale = Vector3.one * penguinSize.FloatValue;

        var statSpeed = new Stat(penguinVelocity.FloatValue);

        var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        penguin.SetCaster(Caster);
        penguin.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * penguinDmg.SetId(Caster.gameObject.name).FloatValue);
        penguin.SetMaxHit(penguinReflect.IntValue > 1 ? penguinReflect.IntValue : 1);
        penguin.SetMaxHitToTarget(1);
        penguin.SetSpeed(statSpeed, new Stat(penguinMoveSpeedInCharge.FloatValue));
        penguin.SetAngle(angle);
        penguin.Play();
    //    penguin.onBeforeDestroy += OnBefore;
        return penguin;
    }

    private void OnBefore(CharacterObjectBase obj)
    {
        try
        {
            listPenguin.Remove(obj as Enemy6001Skill1Object);
        }
        catch (Exception ex)
        {

        }
    }

    private float angle = 0;
    private bool isRotate = false;

    private void FixedUpdate()
    {
        if (!isRotate) return;
        angle += Time.fixedDeltaTime * penguinMoveSpeedInCharge.FloatValue;
        foreach (var penguin in listPenguin)
        {
            penguin.OnUpdate();
        }
        if (angle > penguinReleaseEachAngle.FloatValue)
        {
            Release();
            angle = 0;
        }

    }

    public bool IsCanPlay => listPenguin.Count == 0;

    [Button]
    public void Release()
    {
        if (listPenguin.Count > 0)
        {
            listPenguin[listPenguin.Count - 1].Release();
            listPenguin.RemoveAt(listPenguin.Count - 1);
        }
        else
        {
            isRotate = false;
        }
    }

    public void Reset()
    {
    }
}