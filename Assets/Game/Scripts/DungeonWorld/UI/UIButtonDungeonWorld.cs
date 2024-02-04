using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

public class UIButtonDungeonWorld : UIBaseButton
{
    public override void Action()
    {
        PanelManager.Instance.GetPanel<UIMainMenuPanel>().HideByTransitions().Forget();
        PanelManager.CreateAsync(AddressableName.UIDungeonWorldPanel).ContinueWith(t =>
        {
            var dungeon = DataManager.Save.Dungeon.CurrentDungeon;
            dungeon = Mathf.Clamp(dungeon, 0, DataManager.Base.Dungeon.Dictionary.Count - 2);
            (t as UIDungeonWorldPanel).Show(dungeon);
            t.onClosed += () =>
            {
                PanelManager.Instance.GetPanel<UIMainMenuPanel>().ShowByTransitions();
            };
        }).Forget();
    }
}
