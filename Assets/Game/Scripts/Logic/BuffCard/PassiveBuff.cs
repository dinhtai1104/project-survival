namespace Game.GameActor.Buff
{
    // passive will force apply to caster
    public class PassiveBuff : AbstractBuff
    {
        public override void Play()
        {
            var stats = BuffData.StatRefer.GetAllModifier();
            foreach (var data in stats)
            {
                var stat = data.Value;
                Caster.Stats.AddModifier(data.Key, stat, this);
            }

            if (!string.IsNullOrEmpty(EffectId)) 
            {
                Game.Pool.GameObjectSpawner.Instance.Get(EffectId, obj =>
                 {
                     obj.GetComponent<Effect.EffectAbstract>().Active(Caster.GetPosition()).SetParent(Caster.GetTransform());
                 });
            }
        }
    }
}