using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Talent.UI
{
    public class TalentNotifyCondition : NotifyCondition
    {
        private TalentService _service;
        protected override void Awake()
        {
            base.Awake();
            _service = Architecture.Get<TalentService>();
        }
        public override bool Validate()
        {
            if (_service == null) { return false; }
            return _service.HasNotify();
        }
    }
}
