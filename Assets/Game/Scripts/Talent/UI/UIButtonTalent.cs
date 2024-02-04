using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI;

namespace Assets.Game.Scripts.Talent.UI
{
    public class UIButtonTalent : UIBaseButton
    {
        public override void Action()
        {
            PanelManager.CreateAsync(AddressableName.UITalentPanel).ContinueWith(async t =>
            {
                await UniTask.Yield();
                t.Show();
            }).Forget();
        }
    }
}
