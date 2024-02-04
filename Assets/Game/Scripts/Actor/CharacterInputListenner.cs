using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputListenner : MonoBehaviour, IInputHandler
{
    InputController.ControlAction controlAction;
    private void OnEnable()
    {
        if (controlAction == null)
        {
            Character character = GetComponent<Character>();

            controlAction = new InputController.ControlAction(
                    onTriggered: () =>
                    {
                        Messenger.Broadcast(EventKey.PressKeyControl, EControlCode.Jump, character as ActorBase);
                        character.MoveHandler.Jump();
                    },
                    onReleased: () =>
                    {
                        Messenger.Broadcast(EventKey.ReleaseKeyControl, EControlCode.Jump, character as ActorBase);
                        character.MoveHandler.ReleaseJump();
                    });
        }
        Register();
    }
    private void OnDisable()
    {
        Deregister();
    }
    private void OnDestroy()
    {
        Deregister();
    }
    public void Register()
    {
        Character character = GetComponent<Character>();

       

        GameUIPanel.Instance.inputController.Register(
            character.MoveHandler.Move, 
            character.MoveHandler.Stop,
            //shoot event
            controlAction
            ,
            //reload event
            null
            ,
            null
            //vent event
            // new InputController.ControlAction(
            //    onTriggered: () =>
            //    {
            //        character.Venting();
            //    }, null
            //    )
            )
            ;

    }
    public void Deregister()
    {
        Character character = GetComponent<Character>();
        GameUIPanel.Instance.inputController.Deregister(
            character.MoveHandler.Move,
            character.MoveHandler.Stop,
            //shoot event
            controlAction
            ,
            //reload event
            null
            ,
            null
            //vent event
            // new InputController.ControlAction(
            //    onTriggered: () =>
            //    {
            //        character.Venting();
            //    }, null
            //    )
            )
            ;

    }

    public void Ticks()
    {
    }

    public void Initialize()
    {
    }

    public void SetActive(bool active)
    {
        enabled = active;
    }
}
