using Assets.Game.Scripts._Services;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Logic.DataModel.Noti
{
    public class NotifiQueueCheckFlashSale : NotifiQueueData
    {
        public override bool IsShowOnly => false;
        public NotifiQueueCheckFlashSale(string type) : base(type)
        {
        }

        public async override UniTask Notify()
        {
        }
    }
}
