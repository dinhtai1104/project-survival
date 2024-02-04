using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_7_1
{
    public class Enemy7002Skill3SummonTask : ActorSummonTask
    {
        public LayerMask ground;
        protected override void Spawn(Vector3 pos)
        {
            var levelBuilder = GameController.Instance.GetLevelBuilder();
            var npc = levelBuilder.GetNpcSpawns();
            var npcTarget = npc.GetSpawnPointNearest(Caster.FindClosetTarget().GetPosition());
            pos = npcTarget.Position;
            var rch = Physics2D.Raycast(pos, Vector3.down, 999, ground);
            if (rch.collider)
            {
                pos = rch.point;
            }
            base.Spawn(pos);
        }
    }
}
