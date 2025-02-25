using Gameplay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public abstract class UIGameplayEndGame : GamePanel
    {
        private BaseGameplaySessionSave m_GameSession;

        public BaseGameplaySessionSave GameSession => m_GameSession;
        public override void Show(params object[] @params)
        {
            if (@params.Length > 0)
            {
                m_GameSession = @params[0] as BaseGameplaySessionSave;
            }
            base.Show(@params);
            Setup();
        }

        protected abstract void Setup();
    }
}
