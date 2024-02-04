public class UIButtonSortRarity : UIBaseButton
{
    public UIInventoryBag bag;
    private void Awake()
    {
        bag = GetComponentInParent<UIInventoryBag>();
    }
    public override void Action()
    {
        bag.Show();
        Messenger.Broadcast(EventKey.SortItemRarity);
    }
}
