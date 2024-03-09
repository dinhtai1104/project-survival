using Framework;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class PlayerActor : Actor
    {
        private ActorStatBridge m_StatBridge;
        private WeaponHolderController m_WeaponHolder;
        public WeaponHolderController WeaponHolder => m_WeaponHolder;
        protected override void Awake()
        {
            base.Awake();
            m_WeaponHolder = GetComponent<WeaponHolderController>();
        }
        public override void Init(TeamModel teamModel)
        {
            base.Init(teamModel);
            WeaponHolder.Init(this);
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
