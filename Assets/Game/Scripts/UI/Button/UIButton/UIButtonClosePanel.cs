public class UIButtonClosePanel : UIBaseButton
{
    public UI.Panel panelParent;
    public override void Action()
    {
        if (panelParent == null)
        {
            panelParent = GetComponentInParent<UI.Panel>();
        }
        panelParent.Close();
    }
}
