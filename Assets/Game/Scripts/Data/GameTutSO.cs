using Cysharp.Threading.Tasks;

using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
[CreateAssetMenu(menuName ="Tutorial/GameTut")]
public class GameTutSO : ScriptableObject
{
    GameController gameController;
    private int stepId = 0;
   public void SetUp(GameController gameController)
    {

        this.stepId = 0;
        this.gameController = gameController;

        Messenger.AddListener(EventKey.TutMove, TutMove);
        Messenger.AddListener(EventKey.TutRun, TutRun);

        Messenger.AddListener(EventKey.TutJump, TutJump);
        Messenger.AddListener(EventKey.TutHoldJump, TutHoldJump);

        Messenger.AddListener(EventKey.TutShoot, TutShoot);

        Messenger.AddListener(EventKey.TutJumpPlatform, TutJumpPlatform);
        Messenger.AddListener(EventKey.TutDoubleJump, TutDoubleJump);
        Messenger.AddListener(EventKey.TutDescend, TutDescend);

        Messenger.AddListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.AddListener<bool>(EventKey.StageFinish, OnStageFinish);
    }

    private void OnStageFinish(bool arg1)
    {
        ClearTextTut();
    }

    private void OnGameClear(bool instantClear)
    {
        if (instantClear) return;
        ClearTextTut();
    }

    private void TutDescend()
    {

        SetTutText("Tutorial/TutDescend");


        Messenger.Broadcast(EventKey.ShowHandTut, EHandTut.MovePad);

        Messenger.AddListener(EventKey.CharacterDesend, OnLandedAfterDesend);
        TrackTutorial("Tut Descend");
    }
    private void OnLandedAfterDesend()
    {
        Delay().Forget();
        async UniTask Delay()
        {
            await UniTask.Delay(400);
            ClearTextTut();

            Messenger.Broadcast(EventKey.HideHandTut, EHandTut.MovePad);
            Messenger.RemoveListener(EventKey.CharacterDesend, OnLandedAfterDesend);
        }
        
    }

    private void TutDoubleJump()
    {
        Callback<Callback> callback = null;
        callback = (cb) => WaitForGameReady().Forget();

        Messenger.AddListener<Callback>(EventKey.StageStart, callback);
        async UniTask WaitForGameReady()
        {
            await UniTask.WaitUntil(() => GameUIPanel.Instance.inputController.gameObject.activeInHierarchy);
            SetTutText("Tutorial/TutDoubleJump");


            Messenger.Broadcast(EventKey.ShowHandTut, EHandTut.JumpButton);

            Messenger.AddListener(EventKey.CharacterLanded, OnLanded);

            Messenger.RemoveListener<Callback>(EventKey.StageStart, callback);
            TrackTutorial("Tut DoubleJump");

        }
    }

    private void TutJumpPlatform()
    {
        Callback<Callback> callback = null;
        callback = (cb) => WaitForGameReady().Forget();

        Messenger.AddListener<Callback>(EventKey.StageStart, callback);
        async UniTask WaitForGameReady()
        {
            await UniTask.WaitUntil(() => GameUIPanel.Instance.inputController.gameObject.activeInHierarchy);
            SetTutText("Tutorial/TutJumpPlatform");


            Messenger.Broadcast(EventKey.ShowHandTut, EHandTut.JumpButton);

            Messenger.AddListener(EventKey.CharacterLanded, OnLanded);

            Messenger.RemoveListener<Callback>(EventKey.StageStart, callback);
            TrackTutorial("Tut JumpPlatform");

        }

    }

    private void TutShoot()
    {
        SetTutText("Tutorial/TutShoot");
        TrackTutorial("Tut Shot");
        ActorBase.onDie += OnDie;
    }

    private void OnDie(ActorBase obj, ActorBase attacker)
    {
        ClearTextTut();
        ActorBase.onDie -= OnDie;

    }

    private void TutHoldJump()
    {

        SetTutText("Tutorial/TutHoldJump");

        Messenger.Broadcast(EventKey.ShowHandTut, EHandTut.JumpButton);

        Messenger.AddListener(EventKey.CharacterLanded, OnLanded);

        TrackTutorial("Tut TutHoldJump");
    }

    private void TutJump()
    {
        Callback<Callback> callback = null;
        callback = (cb) => WaitForGameReady().Forget();

        Messenger.AddListener<Callback>(EventKey.StageStart, callback);
        async UniTask WaitForGameReady()
        {
            await UniTask.WaitUntil(() => GameUIPanel.Instance.inputController.gameObject.activeInHierarchy);
            SetTutText("Tutorial/TutJump");


            Messenger.Broadcast(EventKey.ShowHandTut, EHandTut.JumpButton);

            Messenger.AddListener(EventKey.CharacterLanded, OnLanded);

            Messenger.RemoveListener<Callback>(EventKey.StageStart, callback);
            TrackTutorial("Tut Jump");

        }

    }

    private void OnLanded()
    {
        ClearTextTut();
        Messenger.Broadcast(EventKey.HideHandTut, EHandTut.JumpButton);
        Messenger.RemoveListener(EventKey.CharacterLanded, OnLanded);
    }

    private void TutRun()
    {
        SetTutText("Tutorial/TutRun");
        TrackTutorial("Tut Run");
    }

    private void TutMove()
    {
        Callback< Callback> callback = null;
        callback = (cb) => WaitForGameReady().Forget();

        Messenger.AddListener< Callback>(EventKey.StageStart, callback) ;
        async UniTask WaitForGameReady()
        {
            await UniTask.WaitUntil(()=>GameUIPanel.Instance.inputController.gameObject.activeInHierarchy);
            SetTutText("Tutorial/TutMove");

            Messenger.Broadcast(EventKey.ShowHandTut, EHandTut.MovePad);

            Messenger.AddListener<ActorBase>(EventKey.ContinueMovement, OnMove);

            Messenger.RemoveListener<Callback>(EventKey.StageStart, callback);

            TrackTutorial("Tut Move");
        }
    }

    private void TrackTutorial(string stepName)
    {
        FirebaseAnalysticController.Tracker.NewEvent("tutorial")
               .AddIntParam("step_id", this.stepId++)
               .AddStringParam("step_name", stepName)
               .Track();
    }

    private void OnMove(ActorBase actor)
    {
        ClearTextTut();
        Messenger.Broadcast(EventKey.HideHandTut, EHandTut.MovePad);
        Messenger.RemoveListener<ActorBase>(EventKey.ContinueMovement, OnMove);

    }

    UI.Panel tutPanel;
    void ClearTextTut()
    {
        if (tutPanel != null)
        {
            tutPanel.Close();
            tutPanel = null;
        }
    }
    void SetTutText(string textId)
    {
        ClearTextTut();
        UI.PanelManager.Create(AddressableName.UITutorialTextPanel, panel =>
        {
            ((TutorialTextPanel)panel).SetUp(I2Localize.GetLocalize(textId));
            tutPanel = panel;
        });
    }
    public void Clear()
    {
        Messenger.RemoveListener(EventKey.TutMove, TutMove);
        Messenger.RemoveListener(EventKey.TutRun, TutRun);

        Messenger.RemoveListener(EventKey.TutJump, TutJump);
        Messenger.RemoveListener(EventKey.TutHoldJump, TutHoldJump);

        Messenger.RemoveListener(EventKey.TutShoot, TutShoot);

        Messenger.RemoveListener(EventKey.TutJumpPlatform, TutJumpPlatform);
        Messenger.RemoveListener(EventKey.TutDoubleJump, TutDoubleJump);
        Messenger.RemoveListener(EventKey.TutDescend, TutDescend);

        Messenger.RemoveListener<bool>(EventKey.GameClear, OnGameClear);
        Messenger.RemoveListener<bool>(EventKey.StageFinish, OnGameClear);
    }
}
