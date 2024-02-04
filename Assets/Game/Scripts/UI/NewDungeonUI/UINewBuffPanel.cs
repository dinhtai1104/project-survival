using System;
using UnityEngine;

public class UINewBuffPanel : UI.Panel
{
    [SerializeField] private RectTransform contentNewBuff;
    [SerializeField] private UIBuffItemBase buffPrefab;

    public override void PostInit()
    {
        onClosed += OnClose;
    }

    public void Show(int dungeon)
    {
        base.Show();
        var unlockBuffs = DataManager.Base.Buff.BuffUnlockAtDungeon(dungeon);
        foreach (var buff in unlockBuffs)
        {
            var bufIns = PoolManager.Instance.Spawn(buffPrefab, contentNewBuff);
            bufIns.SetEntity(buff);
            bufIns.SetInfor();
        }
    }

    private void OnClose()
    {
        PoolManager.Instance.Clear(buffPrefab.gameObject);
    }
}
