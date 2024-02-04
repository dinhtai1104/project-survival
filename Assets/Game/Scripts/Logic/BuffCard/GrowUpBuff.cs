using Game.GameActor.Buff;
using UnityEngine;

public class GrowUpBuff : AbstractBuff
{
    private float current = 0;
    private float stepIncrease;
    private float maxIncrease;
    private void OnEnable()
    {
        Messenger.AddListener<int, int>(EventKey.GameStart, OnGameStart);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);
    }

    private void OnGameStart(int dungeon, int stage)
    {
        Debug.Log("Pass Stage: " + gameObject.name);
        current += stepIncrease;
        current = Mathf.Clamp(current, 0, maxIncrease);
        if (current < maxIncrease)
        {
            Caster.Stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, stepIncrease), this);
            Game.Pool.GameObjectSpawner.Instance.Get("VFX_DmgBuff", obj =>
             {
                 obj.GetComponent<Game.Effect.EffectAbstract>().Active(Caster.GetPosition()).SetParent(Caster.GetTransform());
             });
        }
    }

    private void OnStageStage(Callback callback)
    {
       
    }

    public override void Play()
    {
        stepIncrease = GetValue(StatKey.MinValue);
        maxIncrease = GetValue(StatKey.MaxValue);
    }
}
