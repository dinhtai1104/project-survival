using System.Collections.Generic;
using UnityEngine;

public class EdgeColliderByPoints : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private EdgeCollider2D colliderTrigger;

    private void Start()
    {
        colliderTrigger = GetComponent<EdgeCollider2D>();
    }

    private void LateUpdate()
    {
        var list = new List<Vector2>();
        list.Add(points[0].localPosition);
        list.Add(points[1].localPosition);
        colliderTrigger.SetPoints(list);
    }
}