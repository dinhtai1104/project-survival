using Cysharp.Threading.Tasks;
using Game.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Assets.Game.Scripts.Logic.Tasks
{
    public class UITaskPlayTweenUI : Task
    {
        public UITweenRunner tween;
        public override async UniTask Begin()
        {
            await base.Begin();
            tween.Show();

            IsCompleted = true;
        }
    }
}
