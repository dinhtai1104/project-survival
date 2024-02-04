using Game.Skill;
using UnityEngine;

public class MultiTaskSkillRandomCooldown : MultiTaskSkill
{
    public ValueConfigSearch minRandom;
    public ValueConfigSearch maxRandom;
    protected override void Start()
    {
        base.Start();
        var random = Random.Range(minRandom.SetId(Caster.gameObject.name).IntValue, maxRandom.IntValue + 1);
        _cooldown.BaseValue = random;
        _cooldown.RecalculateValue();
    }
    protected override void OnCooldownComplete()
    {
        base.OnCooldownComplete();
        var random = Random.Range(minRandom.SetId(Caster.gameObject.name).IntValue, maxRandom.IntValue + 1);
        _cooldown.BaseValue = random;
        _cooldown.RecalculateValue();
    }
}