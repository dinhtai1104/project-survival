using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform:MonoBehaviour
{
}
public class SmallPlatform : Platform
{
    public Transform mid, left, right;
   public void Apply(float width)
    {
        SpriteRenderer sr = mid.GetComponent<SpriteRenderer>();
        sr.size = new Vector2(width,sr.bounds.size.y);
        left.localPosition = new Vector3(-width / 2f, left.localPosition.y);
        right.localPosition = new Vector3(width / 2f, right.localPosition.y);
        GetComponent<BoxCollider2D>().size = new Vector2(width + 3.7f, 0.32f);
    }

    private void OnEnable()
    {
        Apply(GetComponent<BoxCollider2D>().size.x-3.7f);
    }
}
