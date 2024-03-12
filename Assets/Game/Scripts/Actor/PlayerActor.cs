using Assets.Game.Scripts.Core.BuffHandler;
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
        private IBuffEngine m_BuffEngine;
        public IBuffEngine NullBuffEngine;

        public IBuffEngine Buff => m_BuffEngine ?? NullBuffEngine;


        private ActorStatBridge m_StatBridge;
        private WeaponHolderController m_WeaponHolder;
        public WeaponHolderController WeaponHolder => m_WeaponHolder;
        protected override void Awake()
        {
            base.Awake();
            m_BuffEngine = GetComponent<IBuffEngine>();
            m_WeaponHolder = GetComponent<WeaponHolderController>();
        }
        public override void Init(TeamModel teamModel)
        {
            base.Init(teamModel);
            Buff.Init(this);
            WeaponHolder.Init(this);

            m_StatBridge = new ActorStatBridge(this);
        }

        protected override void Update()
        {
            base.Update();
            Buff.OnUpdate();
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
