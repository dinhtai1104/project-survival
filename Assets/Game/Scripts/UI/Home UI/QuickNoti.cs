using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickNoti : UI.Panel
{
    public TMPro.TextMeshProUGUI messageText;
    [SerializeField]
    private AudioClip sfx;
    public override void PostInit()
    {
    }
    public void SetUp(string message)
    {
        Sound.Controller.Instance.PlayOneShot(sfx);
        messageText.text = message;
        Show();
    }
    
}
