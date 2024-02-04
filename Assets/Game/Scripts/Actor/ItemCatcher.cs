using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCatcher : MonoBehaviour
{
    [SerializeField]
    Collector collector;
   
    void OnTriggerEnter2D(Collider2D collider)
    {
        var collectableItem = collider.GetComponent<ICollectable>();
        if (collectableItem != null)
        {
            collectableItem.MoveToward(collector);
        }
    }
}
