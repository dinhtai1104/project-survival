using UnityEngine;

public class HitActionReflectBackToOrigin : MonoBehaviour, IHitTriggerAction
{
    private Vector3 m_Origin;
    private void OnEnable()
    {
        m_Origin = transform.position;
    }
    public void Action(Collider2D collider)
    {
        var dir = transform.right;
        GetComponent<CharacterObjectBase>().Movement.SetDirection(-dir);
    }
}
