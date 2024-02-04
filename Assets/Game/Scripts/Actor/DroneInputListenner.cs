using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Skill;
using InputController;
using UnityEngine;

public class DroneInputListenner : MonoBehaviour, IInputHandler
{
    InputController.ControlAction controlAction;
    [SerializeField]
    private Sprite skillTexture;
    private void OnEnable()
    {
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
    public async UniTask Register()
    {
        Character character = GetComponent<Character>();
        await UniTask.WaitUntil(() => character.IsReady);

        if (controlAction == null)
        {
            controlAction = new InputController.ControlAction(
                    onTriggered: () =>
                    {
                        character.SkillEngine.CastSkill(0);
                    },
                    onReleased: () =>
                    {
                    },
                    new InputController.CoolDownConfig(
                        triggerCoolDownCondition: new WaitForSkillCoolDown(character),
                        deactiveCondition: new WaitForSkillEmpty(character)
                        ,
                        skill: character.SkillEngine.GetSkill(0))
                        ,
                    /*((MultiTaskSkill)character.SkillEngine.GetSkill(0)).maxCastTime.IntValue!=0?new TrackSkillConfig(character,true):null*/ null,
                    texture: skillTexture);
        }
        GameUIPanel.Instance.inputController.RegisterControlAction(
            // 2nd button
            index:1,
            //shoot event
            controlAction
            );

    }
    public void  Deregister()
    {

        GameUIPanel.Instance.inputController.DeregisterControlAction(
            // 2nd button
            index: 1,
            //shoot event
            controlAction
            );

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
