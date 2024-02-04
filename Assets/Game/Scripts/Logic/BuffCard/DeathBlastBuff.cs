using Game.GameActor;
using Game.GameActor.Buff;
using UnityEngine;

namespace Game.BuffCard
{
    public class DeathBlastBuff : AbstractBuff
    {
        public ValueConfigSearch deathBlast_Radius = new ValueConfigSearch("Buff_DeathBlast_RadiusExplosion");
        [SerializeField] private DamageExplode damageExplode;
        private float sizeExplodeArea;
        private float dmgMul;
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.KilledBy, OnKill);
        }

        private void OnKill(ActorBase attacker, ActorBase defender)
        {
            if (Caster != attacker) return;
            // spawn explode
            var posDeath = defender.GetPosition();
            var explode = PoolManager.Instance.Spawn(damageExplode);
            explode.transform.position = posDeath;
            explode.Init(Caster);
            explode.SetSize(sizeExplodeArea);
            explode.SetDmg(dmgMul * attacker.Stats.GetValue(StatKey.Dmg));
            explode.Explode();
        }

        public override void Play()
        {
            sizeExplodeArea = deathBlast_Radius.FloatValue;
            dmgMul = GetValue(StatKey.Dmg);
        }
    }
}