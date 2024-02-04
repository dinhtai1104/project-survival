using com.mec;
using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

public class ApplyAutoDestroyActor : MonoBehaviour
{
    public ValueConfigSearch TimeDestroy;
    private void OnEnable()
    {
        Timing.RunCoroutine(_AutoDestroy(GetComponent<ActorBase>()), gameObject);
    }
    private IEnumerator<float> _AutoDestroy(ActorBase owner)
    {
        yield return Timing.WaitForSeconds(TimeDestroy.FloatValue + 1);
        if (!owner.IsDead())
        {
            //owner.IsActived = false;
            //owner.gameObject.SetActive(false);
            Messenger.Broadcast<ActorBase>(EventKey.AutoDestroyActor, owner);
            owner.DeadForce();
        }
    }

    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }
}
