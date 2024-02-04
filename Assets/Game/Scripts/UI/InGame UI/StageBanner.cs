using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBanner : UI.Panel
{
    public static StageBanner Instance;
    [SerializeField]
    private TMPro.TextMeshProUGUI stageText;
    [SerializeField]
    private AudioClip sfx;
    public override void PostInit()
    {
        Instance = this;
    }

    public void SetUp(int stage)
    {
        Sound.Controller.Instance.PlayOneShot(sfx, 1);
        Show();
    }
}
