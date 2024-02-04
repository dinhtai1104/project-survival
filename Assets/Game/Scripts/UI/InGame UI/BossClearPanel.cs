using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossClearPanel : UI.Panel
{
    public static BossClearPanel Instance;
    public override void PostInit()
    {
        Instance = this;
    }

    public void SetUp()
    {
        Show();
    }
}
