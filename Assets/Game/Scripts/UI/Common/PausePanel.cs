using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : UI.Panel
{
    public static PausePanel Instance;
    public TMPro.TMP_InputField[] inputs;

    public override void PostInit()
    {
        Instance = this;
    }
    string gravity, jumpForce, moveSpeed, climbDrag, cameraSize, offset;
    public void SetUp()
    {
    

        Show();
    }

    public void Apply()
    {
        //PlayerController pc = (PlayerController)((BattleGameController)Game.Controller.Instance.gameController).player;

        //pc.GetRigidbody().gravityScale = float.Parse(inputs[0].text);
        //((PlayerMoveHandler)(pc.MoveHandler)).jumpForce = float.Parse(inputs[1].text);
        //pc.MoveHandler.MoveSpeed = float.Parse(inputs[2].text);
        //((PlayerMoveHandler)(pc.MoveHandler)).climbDrag = float.Parse(inputs[3].text);

        //CameraController.Instance.SetCameraSize(float.Parse(inputs[4].text));
        //CameraController.Instance.SetOffset(new Vector3(float.Parse(inputs[5].text), float.Parse(inputs[6].text),-25));

    }
}
