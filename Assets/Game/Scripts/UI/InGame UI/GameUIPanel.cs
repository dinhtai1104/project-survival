using Cysharp.Threading.Tasks;
using Game;
using Game.GameActor;
using GameUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIPanel : UI.Panel
{
    private const string ZERO = "0";
    private static GameUIPanel instance;
    public static GameUIPanel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameUIPanel>();
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    GameController gameController;
    public InputController.InputController inputController;
    [SerializeField]
    private TMPro.TextMeshProUGUI stageText;
    public GameObject pauseBtn;
    public GameObject[] moveTutObjs,shootTutObjs,combatTutObjs;

    int totalAlive = 9;
   
    public void SetUp(GameController gameController)
    {
        this.gameController = gameController;
        if (gameController.GetSession() != null)
        {
            stageText.SetText(string.Format(I2Localize.GetLocalize("Common/Title_Stage"), $"{gameController.GetSession().CurrentStage + 1}/{gameController.GetDungeonEntity().Stages.Count}"));
        }
        Show();
    }
  
  
   
    void OnDie(ActorBase pointBase, ActorBase damageSource)
    {
        totalAlive--;
    }


    private void OnDisable()
    {
        ActorBase.onDie -= OnDie;

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        ActorBase.onDie -= OnDie;
        instance = null;
    }
    public override void Clear()
    {
        base.Clear();
        ActorBase.onDie -= OnDie;
        instance = null;
    }

    public void ShowSettings()
    {
        PausePanel.Instance.SetUp();
    }

    public override void PostInit()
    {
        Instance = this;
    }
}
