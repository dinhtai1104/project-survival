using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_7_2
{
    [CreateAssetMenu(menuName = "CharacterBehaviours/Summon7002Behaviour")]
    public class Summon7002Behaviour : CharacterBehaviour
    {
        private float time = 0;
        public ValueConfigSearch cooldown;
        public string enemyId;
        public ValueConfigSearch number;
        private ActorBase actor;

        public override CharacterBehaviour SetUp(ActorBase character)
        {
            var bh = (Summon7002Behaviour)base.SetUp(character);
            bh.cooldown = cooldown;
            bh.actor = character;
            bh.enemyId = enemyId;
            bh.number = number;
            bh.ground = ground;
            return bh;
        }
        public override void OnActive(ActorBase character)
        {
            base.OnActive(character);
            active = true;
            time = 0;
        }

        public override void OnUpdate(ActorBase character)
        {
            base.OnUpdate(character);
            if (character.Tagger.HasAnyTags(ETag.Stun)) return;
            time += Time.deltaTime;
            if (time > cooldown.FloatValue)
            {
                time = 0;
                Summon();
            }
        }

        private void Summon()
        {
            Spawn();
        }
        public LayerMask ground;
        protected void Spawn()
        {
            if (actor.FindClosetTarget() == null) return;
            var levelBuilder = GameController.Instance.GetLevelBuilder();
            var npc = levelBuilder.GetNpcSpawns();
            var npcTarget = npc.GetSpawnPointNearest(this.actor.FindClosetTarget().GetPosition());
            var pos = npcTarget.Position;
            var rch = Physics2D.Raycast(pos, Vector3.down, 999, ground);
            if (rch.collider)
            {
                pos = rch.point;
            }
            var enemyhandler = GameController.Instance.GetEnemySpawnHandler();
            enemyhandler.SpawnSingle(enemyId, (int)actor.GetStatValue(StatKey.Level), pos, usePortal: true).Forget();
        }
    }
}
