using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour,IBatchUpdate
{
  

    public Character character;
    Transform t;
    [SerializeField]
    private PointEffector2D magnetEffector;
    [SerializeField]
    private CircleCollider2D magnetCollider;
    private float defaultPower, defaultRadius;

    
    public void SetUp(Character character,float magnetPowerMultiplier=1,float magnetRadiusMultiplier=1f)
    {
        this.character = character;
        t = transform;
     
        gameObject.SetActive(true);
    //    magnetEffector.forceMagnitude =defaultPower* magnetPowerMultiplier;
  //      magnetCollider.radius =defaultRadius* magnetRadiusMultiplier;

    }
    private void OnEnable()
    {
        //UpdateSlicer.Instance.RegisterSlicedUpdate(this);
        //defaultPower = magnetEffector.forceMagnitude;
        //defaultRadius = magnetCollider.radius;
        SetUp(GetComponentInParent<Character>(), 1, 1);

    }
    private void OnDisable()
    {
        //UpdateSlicer.Instance.DeregisterSlicedUpdate(this);
      //  magnetEffector.forceMagnitude = defaultPower ;
     //   magnetCollider.radius = defaultRadius;
    }

    public void BatchUpdate()
    {
        //t.localPosition = character.GetMidTransform().position;
    }

    public void BatchFixedUpdate()
    {
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (character==null ||character.IsDead()) return;

        var collectableItem = collider.GetComponent<ICollectable>();
        if (collectableItem != null)
        {
            collectableItem.Collect(this);
        }
    }
}
