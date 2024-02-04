using System.Collections.Generic;
using UnityEngine;

public class UITabControl : MonoBehaviour
{
    [SerializeField] private List<UIButtonTab> uiButtonTab = new List<UIButtonTab>();
    [SerializeField] private List<UIContentTab> uiContentTab = new List<UIContentTab>();

    private void OnEnable()
    {
        for (int i = 0; i < uiButtonTab.Count; i++)
        {
            uiButtonTab[i].Register(this, uiContentTab[i]);
        }

        SetTab(uiButtonTab[0], uiContentTab[0]);
    }

    public void SetTab(UIButtonTab button, UIContentTab tab)
    {
        foreach (var btn in uiButtonTab)
        {
            btn.SetActive(false);
        }
        foreach (var tb in uiContentTab)
        {
            tb.gameObject.SetActive(false);
        }
        button.SetActive(true);
        tab.gameObject.SetActive(true);
        tab.Show();
    }
}