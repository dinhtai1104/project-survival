using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdOfferObj : MonoBehaviour
{
    public void Trigger()
    {
        UI.PanelManager.CreateAsync<UIAdOfferPanel>(AddressableName.UIAdOfferPanel).ContinueWith(panel=> 
        {
            panel.Show();
        }).Forget();
    }
}
