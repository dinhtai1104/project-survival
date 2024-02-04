using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOnDeadEventCaller : MonoBehaviour
{
    private Character m_Character;
    private void Awake()
    {
        m_Character = GetComponentInParent<Character>();
    }
    public void OnAction()
    {
        m_Character.OnDead();
    }
}
