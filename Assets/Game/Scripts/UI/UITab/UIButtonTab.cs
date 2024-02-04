using UnityEngine;
using UnityEngine.UI;

public class UIButtonTab : UIBaseButton
{
    private UITabControl control;
    private UIContentTab tab;
    [SerializeField] private Sprite onSpr, offSpr;
    public bool IsSetNative = false;
    public void Register(UITabControl control, UIContentTab tab)
    {
        this.control = control;
        this.tab = tab;
    }
    public override void Action()
    {
        control.SetTab(this, tab);
    }
    public void SetActive(bool active)
    {
        button.image.sprite = active ? onSpr : offSpr;
        if (IsSetNative)
        {
            button.image.SetNativeSize();
        }
    }
}
