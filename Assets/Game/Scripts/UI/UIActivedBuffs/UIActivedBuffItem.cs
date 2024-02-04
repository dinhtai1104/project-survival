using ExtensionKit;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIActivedBuffItem : UIBuffItemBase
{ 
    public async void OnClickedBuff()
    {
        var tooltip = await PanelManager.CreateAsync<UITooltipPanel>(AddressableName.UITooltipPanel);
        var data = new BuffData(buffEntity.Type);
        tooltip.Show(data);
    }
    public override void SetInfor()
    {
        base.SetInfor();
        try
        {
            if (SceneManager.GetActiveScene().name == "MainScene") return;
            var session = GameController.Instance.GetSession();
            var buff = session.buffSession.Dungeon.BuffEquiped[buffEntity.Type];
            if (!buff.CanActiveAgain)
            {
                Color c = "#737373".HtmlStringToColor();
                buffOutlineImg.color = buffIconImg.color = c;
            }
            else
            {
                buffOutlineImg.color = buffIconImg.color = Color.white;
            }

        }
        catch (System.Exception e)
        {

        }
    }
}