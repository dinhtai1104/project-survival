using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.BattlePass.UI;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using UI;
using UnityEngine;

[System.Serializable]
public class BattlePassService : Service
{
    [SerializeField] private ExpHandler expHandler;
    [SerializeField] private BattlePassSaves save;
    [SerializeField] private BattlePassTable table;
    public int Level => expHandler.CurrentLevel;
    public bool IsPremium => Save.ActivePremium;
    public int Season => Save.Season;
    public DateTime TimeEnd => Save.TimeEnd;
    public float ProgressExp => expHandler.LevelProgress;
    public bool IsRunning => Save.IsRunning;

    public BattlePassSaves Save { get => save; set => save = value; }
    public BattlePassTable Table { get => table; set => table = value; }

    public delegate void OnBattlePassClaim(int level);
    public delegate void OnBattlePassAddExp(int exp);
    public event OnBattlePassClaim BattlePassClaimEvent;
    public event OnBattlePassAddExp BattlePassAddExpEvent;

    public override void OnInit()
    {
        base.OnInit();
        Save = DataManager.Save.BattlePass;
        Table = DataManager.Base.BattlePass;
        var exp = Table.GetAllExp();
        expHandler = new ExpHandler(Save.Exp, exp);
        expHandler.OnLevelChanged += OnLevelChange;
    }

    private void OnLevelChange(int from, int to)
    {
        if (from == 0 && to >= 1)
        {
            save.ActiveBattlePass();
        }

        Architecture.Get<NotifiQueueService>().EnQueue(new NotifiQueueBattlePassLevelUp
        {
            Type = "BattlePass_Level",
            Level = to
        });
    }


    public void AddExp(long exp)
    {
        if (IsRunning)
        {
            Save.AddExp(exp);
            expHandler.Add(exp);
            BattlePassAddExpEvent?.Invoke((int)exp);
        }
    }

    public void ClaimBattlePass(int level, EBattlePass type)
    {
        Save.BuyBattlePass(level, type);
        BattlePassClaimEvent?.Invoke(level);
    }

    public override void OnDispose()
    {
        base.OnDispose();
        expHandler.OnLevelChanged -= OnLevelChange;
    }

    public BattlePassSave GetBattlePass(int level)
    {
        return Save.GetBattlePass(level);
    }

    [Button]
    public void AddExp(int exp)
    {
        Save.AddExp(exp);
        expHandler.Add(exp);
        BattlePassAddExpEvent?.Invoke(exp);
    }

    public bool IsClaimed(int level, EBattlePass type)
    {
        return Save.IsClaimed(level, type);
    }

    public void BuyPremium()
    {
        save.BuyPremium();
        Messenger.Broadcast(EventKey.BuyPremiumBattlePass);
    }

    public void Reset()
    {
        save.Fix();
    }

    public void ClaimPremiumDaily()
    {
        save.CanClaimPremium = false;
        save.Save();
    }

    public bool CanRewardDaily()
    {
        return save.CanClaimPremium;
    }

    public float GetExtraFragmentDrop()
    {
        if (!IsPremium) return 0;
        return 1f;
    }

    public float GetExtraStoneDrop()
    {
        if (!IsPremium) return 0;
        return 1f;
    }

}