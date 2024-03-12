using Assets.Game.Scripts.Core.Data.Database;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class EnemyActor : Actor
    {
        public int MonsterLevel { set; get; }
        public EnemyEntity EntityData { set; get; }
        private ActorStatBridge m_StatBridge;

        public override void Init(TeamModel teamModel)
        {
            base.Init(teamModel);
            m_StatBridge?.Dispose();
            m_StatBridge = new ActorStatBridge(this);
        }
        public override void Reset()
        {
            base.Reset();
            m_StatBridge?.Reset();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_StatBridge?.Dispose();
        }
    }
}
