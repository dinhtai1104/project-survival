using Gameplay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class UIGameplayEndGame : GamePanel
    {
        private BaseGameplaySession m_GameSession;

        public BaseGameplaySession GameSession => m_GameSession;
        public override void Show(params object[] @params)
        {
            base.Show(@params);
            if (@params.Length > 0)
            {
                m_GameSession = @params[0] as BaseGameplaySession;
            }
        }
    }
}
