using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    System.Action onTriggered;
    bool isTriggered = false;
    [SerializeField]
    private UnityEngine.Events.UnityEvent onPointTriggered;
    public void Register(Vector3 pos,System.Action onTriggered)
    {
        transform.position = pos;
        isTriggered = false;
        this.onTriggered = onTriggered;
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        isTriggered = false;
    }
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;
        ActorBase actor = collision.GetComponent<ActorBase>();
        if (actor==Game.Controller.Instance.gameController.GetMainActor())
        {
            isTriggered = true;

            onTriggered?.Invoke();
            onPointTriggered?.Invoke();
            onTriggered = null;
        }
    }
}
