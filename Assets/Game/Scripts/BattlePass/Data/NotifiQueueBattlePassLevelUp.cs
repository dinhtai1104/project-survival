using Assets.Game.Scripts.BattlePass.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace Assets.Game.Scripts._Services
{
    public class NotifiQueueBattlePassLevelUp : NotifiQueueData
    {
        public int Level;
        public override async UniTask Notify()
        {
            bool close = false;
            var ui = await PanelManager.CreateAsync<UIBattlePassLevelUpPanel>(AddressableName.UIBattlePassLevelUpPanel);
            ui.Show(0, Level);

            ui.onClosed += () => close = true;
            await UniTask.WaitUntil(() => close);
        }
    }
}
