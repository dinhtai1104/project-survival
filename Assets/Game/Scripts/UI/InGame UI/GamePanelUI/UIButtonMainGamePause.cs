using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIButtonMainGamePause : UIBaseButton
{
    public override async void Action()
    {
        var notice = (await UI.PanelManager.CreateAsync<UIPausePanel>(AddressableName.UIPausePanel));
        notice.Show();
        Time.timeScale = 0;
    }
}