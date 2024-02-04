using Cysharp.Threading.Tasks;
using System;
using UI;

namespace Assets.Game.Scripts.Logic.Memory
{
    public class MenuViewActionMemory : ISceneMemory
    {
        public string address;

        public async UniTask<Panel> ActionMemory()
        {
            var ui = await PanelManager.CreateAsync(address);
            return ui;
        }
    }
}
