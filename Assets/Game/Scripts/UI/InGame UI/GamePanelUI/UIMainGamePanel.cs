public class UIMainGamePanel : UI.Panel
{
    public UIPlayerInfoView viewHero;
    public override void PostInit()
    {
    }
    public override void Show()
    {
        base.Show();
        viewHero.Setup();
    }
}
