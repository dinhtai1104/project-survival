using Game.GameActor;
using UnityEngine;

public class HitActionSetStatus : MonoBehaviour, IHitTriggerAction
{
    public EStatus statusPrefab;
    public ValueConfigSearch durationStatus;
    private CharacterObjectBase characterObject => GetComponent<CharacterObjectBase>();
    public async void Action(Collider2D collider)
    {
        durationStatus = durationStatus.SetId(characterObject.Caster.gameObject.name);
        var actor = collider.GetComponentInParent<ActorBase>();
        if (actor != characterObject.Caster && actor != null)
        {
            var status = await actor.StatusEngine.AddStatus(characterObject.Caster, statusPrefab, this);
            if (status == null) return;
            status.Init(characterObject.Caster, actor);
            status.SetDuration(durationStatus.FloatValue);
        }
    }
}
