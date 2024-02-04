
using Game.GameActor;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterBehaviours/HealDroneGUI")]
public class HealDroneGUI : CharacterBehaviour
{

    public Vector3 offset=new Vector3(-2,2.5f);
    HealDroneUI ui;

    public override CharacterBehaviour SetUp(ActorBase character)
    {
        HealDroneGUI instance = (HealDroneGUI)base.SetUp(character);
        instance.offset = this.offset;

        return instance;
    }
    public override void OnActive(ActorBase character)
    {
        active = true;
        Game.Pool.GameObjectSpawner.Instance.Get("HealDroneUI", obj =>
        {
             ui = obj.GetComponent<HealDroneUI>();

            ui.SetUp(character, character.GetTransform(), offset);

        }, Game.Pool.EPool.Pernament);
        Messenger.AddListener(EventKey.PlayerTeleported, OnGameClear);
        Messenger.AddListener<Callback>(EventKey.StageStart, OnGameStart);
    }

    private void OnGameStart(Callback arg1)
    {
        if (ui!=null)
        {
            ui.SetActive(true);
        }
    }

   
    private void OnGameClear()
    {
        if (ui != null)
        {
            ui.SetActive(false);
        }
    }

    public override void OnDeactive(ActorBase character)
    {
        base.OnDeactive(character);
        Messenger.RemoveListener(EventKey.PlayerTeleported, OnGameClear);
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);

    }




}
