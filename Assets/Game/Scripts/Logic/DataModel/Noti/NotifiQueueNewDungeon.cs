using Assets.Game.Scripts._Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace Assets.Game.Scripts.Logic.DataModel.Noti
{
    public class NotifiQueueNewDungeon : NotifiQueueData
    {
        public override bool IsShowOnly => false;
        public NotifiQueueNewDungeon(string type) : base(type)
        {
        }

        public async override UniTask Notify()
        {
            var dungeonSave = DataManager.Save.Dungeon;
            var dungeonTable = DataManager.Base.Dungeon.Dictionary.Count - 1;
            int dungeon = dungeonSave.CurrentDungeon;
            if (dungeonSave.IsShowAnimDungeon(dungeon)) return;
            for (int i = 0; i < dungeon; i++)
            {
                dungeonSave.ShowAnimDungeon(i);
            }

            if (dungeon >= dungeonTable) return;
            // Show
            var ui = await PanelManager.CreateAsync<UINewDungeonPanel>(AddressableName.UINewDungeonPanel);
            bool wait = false;
            ui.Show(dungeon);
            ui.onClosed += () => wait = true;
            await UniTask.WaitUntil(() => wait);

            dungeonSave.ShowAnimDungeon(dungeon);
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
        }
    }
}
