using com.mec;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class HitActionBounceObject : MonoBehaviour, IHitTriggerAction
{
    public ValueConfigSearch MaxReflect;
    private CharacterObjectBase ObjectBase;
    private bool cooldownBounce = false;
    [SerializeField] public LayerMask bounceLayer;

    private List<Vector3> listDirectionWillChange = new List<Vector3>();
    private int indexReflect = 0;

    private void Start()
    {
    }
    private void OnEnable()
    {
        ObjectBase = GetComponent<CharacterObjectBase>();
        ObjectBase.onBeforePlay += OnBeforePlay;

    }
    private void OnDisable()
    {
        ObjectBase.onBeforePlay -= OnBeforePlay;
    }

    private void OnBeforePlay()
    {
        indexReflect = 0;
        //listDirectionWillChange = new List<Vector3>();
        //var starDir = transform.right;
        //var ray = new Ray(transform.position, transform.right);
        //int max = MaxReflect.IntValue;
        //var hit = Physics2D.Raycast(ray.origin, ray.direction, 30, bounceLayer);
        //for (int i = 0; i < max; i++)
        //{
        //    hit = Physics2D.Raycast(ray.origin, ray.direction, 30, bounceLayer);
        //    if (hit)
        //    {
        //        listDirectionWillChange.Add(Vector2.Reflect(ray.direction, hit.normal));
        //        ray = new Ray(hit.point - (Vector2)ray.direction * 0.1f, Vector2.Reflect(ray.direction, hit.normal));
        //    }
        //}
    }

    public void Action(Collider2D collider)
    {
        if (HasLayer(collider.gameObject.layer))
        {
            //if (cooldownBounce)
            //    return;
            if (indexReflect >= MaxReflect.SetId(ObjectBase.Caster.gameObject.name).IntValue) 
            {
                //PoolManager.Instance.Despawn(gameObject);
                return; 
            }
            //cooldownBounce = true;

            //var newDir = listDirectionWillChange[indexReflect++];
            var hit = Physics2D.Raycast(transform.position, transform.right, 5, layerMask: bounceLayer);

            transform.right = Vector3.Reflect(transform.right,hit.normal);

            ObjectBase.Movement.SetDirection(transform.right);

            indexReflect++;
            Timing.RunCoroutine(_Cooldown());
        }
    }

    private IEnumerator<float> _Cooldown()
    {
        yield return Timing.WaitForSeconds(0.1f);
        cooldownBounce = false;
    }

    protected bool HasLayer(int layer)
    {
        return ((1 << layer) & bounceLayer) != 0;
    }
}
