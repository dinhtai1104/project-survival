using System.Collections.Generic;
using UnityEngine;

public class LineControlByPoint : MonoBehaviour
{
    [SerializeField] private Transform p0;
    [SerializeField] private Transform p1;
    [SerializeField] private LayerMask layerMask;

    private void Update()
    {
        var hit = Physics2D.Raycast(p0.position, transform.right, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            p1.position = hit.point;
            return;
        }
        p1.position = p0.position + transform.right * 50;
    }
    public float GetLength()
    {
        return (p1.position - p0.position).magnitude;
    }
}