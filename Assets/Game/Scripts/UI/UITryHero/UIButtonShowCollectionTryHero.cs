public class UIButtonShowCollectionTryHero : UIBaseButton
{
    public override async void Action()
    {
        var ui = await UI.PanelManager.CreateAsync<UITryHeroCollectionsPanel>(AddressableName.UITryHeroCollectionsPanel);
        ui.Show();
    }
}
