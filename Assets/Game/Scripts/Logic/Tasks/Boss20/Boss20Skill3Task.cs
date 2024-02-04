using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss20Skill3Task : SkillTask
{
    [SerializeField] private string eff_Spin = "VFX_Boss20_Spin";
    [SerializeField] private Transform posEff_Spin;
    public string _animationMainSkill;
    [SerializeField] private ValueConfigSearch leafDmg;
    [SerializeField] private ValueConfigSearch timwBtwShootLeaf;
    [SerializeField] private ValueConfigSearch durationSkill;
    [SerializeField] private ValueConfigSearch leafVelocity;

    [SerializeField] private Transform posSpawn;
    [SerializeField] private GameObject leafPrefab;
    private List<GameObject> objectSkills = new List<GameObject>();
    private CoroutineHandle handle;
    private bool isRun = false;
    public override async UniTask Begin()
    {
        await base.Begin();
        objectSkills.Clear();
        Caster.AnimationHandler.SetAnimation(_animationMainSkill, true);
        Caster.AnimationHandler.onEventTracking += OnEventTracking;

        duration = durationSkill.FloatValue;
        time = 0;
        isRun = true;

        //handle = Timing.RunCoroutine(_ShootLeaf(), Segment.FixedUpdate, gameObject);
    }
    public override UniTask End()
    {
        isRun = false;
        Caster.AnimationHandler.onEventTracking -= OnEventTracking;
        return base.End();
    }

    private async void OnEventTracking(TrackEntry trackEntry, Spine.Event e)
    {
        if (trackEntry.Animation.Name == _animationMainSkill)
        {
            if (e.Data.Name == "attack_tracking")
            {
                var effPrefab = await ResourcesLoader.Instance.LoadAsync<GameObject>(eff_Spin);
                var eff = PoolManager.Instance.Spawn(effPrefab);
                eff.transform.position = posEff_Spin.transform.position;
                eff.transform.localScale = new Vector3(Mathf.Sign(Caster.GetLookDirection().x), 1, 1);
            }
        }
    }
    float duration;
    float time = 0;
    private IEnumerator<float> _ShootLeaf()
    {
        float duration = durationSkill.FloatValue;
        float time = 0;
        while (duration > 0)
        {
            if (time <= 0)
            {
                ShootLeaf();
                time = timwBtwShootLeaf.FloatValue;
            }
            time -= Time.fixedDeltaTime;
            duration -= Time.fixedDeltaTime;
            yield return Timing.WaitForSeconds(Time.fixedDeltaTime);
        }
        //DestroyAllLeafAvailable();
        IsCompleted = true;
        Timing.KillCoroutines(handle);
    }

    private void FixedUpdate()
    {
        if (!isRun) return;

        if (duration > 0)
        {
            if (time <= 0)
            {
                ShootLeaf();
                time = timwBtwShootLeaf.FloatValue;
            }
            time -= Time.fixedDeltaTime;
            duration -= Time.fixedDeltaTime;
        }
        else
        {
            //DestroyAllLeafAvailable();
            IsCompleted = true;
        }
    }

    public override void OnStop()
    {
        base.OnStop();
        End();
        isRun = false;
        Timing.KillCoroutines(gameObject);
       // DestroyAllLeafAvailable();
    }

    private void DestroyAllLeafAvailable()
    {
        foreach (var i in objectSkills)
        {
            if (i != null)
            {
                if (i.gameObject.activeSelf)
                {
                    PoolManager.Instance.Despawn(i.gameObject);
                }
            }
        }
        objectSkills.Clear();
    }
    public override void UnityDisable()
    {
        Timing.KillCoroutines(handle);
        base.UnityDisable();
    }
    private void ShootLeaf()
    {
        if (posSpawn != null)
        {
            var leaf = PoolManager.Instance.Spawn(leafPrefab).GetComponent<Boss20Skill3Leaf>();
            leaf.transform.SetPositionAndRotation(posSpawn.position, posSpawn.rotation);
            leaf.GetComponent<CharacterObjectBase>().SetCaster(Caster);
            var statSpeed = new Stat(leafVelocity.FloatValue);

            var listModi = new List<ModifierSource>() { new ModifierSource(statSpeed) };
            Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi); 
            leaf.Movement.Speed = statSpeed;
            leaf.GetComponent<IDamage>().DmgStat = new Stat(leafDmg.FloatValue * Caster.Stats.GetValue(StatKey.Dmg));
            leaf.Play();

            objectSkills.Add(leaf.gameObject);
        }
    }
}