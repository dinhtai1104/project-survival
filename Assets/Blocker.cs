using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour, ITarget
{
    public bool CanFocusOn()
    {
        return false;
    }

    public ECharacterType GetCharacterType()
    {
        return ECharacterType.Object;
    }

    public float GetHealthPoint()
    {
        return 0;
    }

    public bool GetHit(DamageSource damageSource, IDamageDealer dealer)
    {
        return false;
    }

    public Vector3 GetMidPos()
    {
        return GetMidTransform().position;
    }

    public Transform GetMidTransform()
    {
        return GetTransform();
    }

    public Vector3 GetPosition()
    {
        return GetMidPos();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public void HighLight(bool active)
    {
    }

    public bool IsDead()
    {
        return false;
    }

    public bool IsThreat()
    {
        return false;
    }
}
