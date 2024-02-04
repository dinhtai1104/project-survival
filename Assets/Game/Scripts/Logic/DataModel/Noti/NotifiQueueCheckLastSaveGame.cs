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
    public class NotifiQueueCheckLastSaveGame : NotifiQueueData
    {
        private bool IsClickBackLastGame = false;
        public NotifiQueueCheckLastSaveGame(string type) : base(type)
        {
        }

        public async override UniTask Notify()
        {
        }
    }
}
