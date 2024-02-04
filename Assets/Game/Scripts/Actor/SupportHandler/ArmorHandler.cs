using Game.GameActor;
using GameUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorHandler : IArmorHandler
{
    public Character character;
    public bool active;
    public void SetUp(Character character)
    {
        this.character = character;
        active = true;
        character.HealthHandler.onArmorBroke += Break;

    }
    public void Break()
    {
        active = false;
    }

    public void GetHit()
    {
    }
}
