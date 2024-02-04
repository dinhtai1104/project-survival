using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using UnityEngine.UI;

public class UICheatInGame : MainMenuCheat
{
    private bool isImmune = false;
    public Toggle toggle;
#if DEVELOPMENT || UNITY_EDITOR

    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnImmuneCallback);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnImmuneCallback);
    }


    protected override void Init()
    {
        toggle = GetComponentInChildren<Toggle>();
        toggle.onValueChanged.RemoveListener(OnValueChange);
        toggle.onValueChanged.AddListener(OnValueChange);
        AddButton("Skip tutorial", OnSkipTutorial);
        AddButton("Stop Shot", OnStopShoot);
        AddButton("Continue Shot", OnContinueShoot);
        AddButton("Dead", OnDead);
        //AddButton("Kill Enemies", OnKillEnemies);
        AddButton("Heal", OnHeal);
        AddButton("Immune", OnImmune);
        AddButton("Pass Level", OnPassLevel);
    }

    private void OnImmune()
    {
        isImmune = !isImmune;
    }
    void OnImmuneCallback(ActorBase attacker, ActorBase defender, DamageSource dmg)
    {
        if (!isImmune) { return; }
        if (defender == GameController.Instance.GetMainActor())
        {
            dmg._damage = new Stat(0);
        }
    }

    private void OnValueChange(bool active)
    {
        try
        {
            if (active)
            {
                (GameController.Instance as BattleGameController).ShowGUI();
            }
            else
            {
                (GameController.Instance as BattleGameController).HideGUI();
            }
        }
        catch (Exception e)
        {

        }
    }

    private void OnPassLevel()
    {
        GameController.Instance.GetEnemySpawnHandler().ClearAllEnemy();
        //Messenger.Broadcast(EventKey.StageFinish, false);
    }

    private void OnHeal()
    {
        GameController.Instance.GetMainActor().Heal(99999);
    }

    private void OnKillEnemies()
    {
        for (int i = 0; i < GameController.Instance.GetEnemySpawnHandler().enemies.Count; i++)
        {
            ((Character)GameController.Instance.GetEnemySpawnHandler().enemies[i]).Dead();
        }
    }

    private void OnDead()
    {
#if DEVELOPMENT
        GameController.Instance.GetMainActor().DeadForceTest();
#endif
    }

    private void OnContinueShoot()
    {
        GameController.Instance.GetMainActor().AttackHandler.active = true;
    }

    private void OnStopShoot()
    {
        GameController.Instance.GetMainActor().AttackHandler.active = false;
    }

    private void OnSkipTutorial()
    {
        DataManager.Save.General.IsGameTutFinished = true;
        DataManager.Save.General.Save();
        Game.Controller.Instance.LoadMenuScene().Forget();
    }
#endif
    }