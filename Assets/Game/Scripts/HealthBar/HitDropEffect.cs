using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitDropEffect : MonoBehaviour
{
    RectTransform _transform;
    Image img;
    float timer = 0;
    Vector2 move;
    Color color;
    public float DropSpeed = 10,DropTime=0.25f;
    public AnimationCurve DropCurve,FadeCurve;
    public void SetUp(Vector2 position,float width)
    {
        _transform = GetComponent<RectTransform>();
        img = GetComponent<Image>();
        _transform.anchoredPosition = position;
        _transform.sizeDelta = new Vector2(width,_transform.sizeDelta.y);
        timer = 0;

        move.y = DropSpeed;
        color = img.color;
        color.a = 1;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        if (timer < DropTime)
        {
            _transform.anchoredPosition += move*(DropCurve.Evaluate(timer/DropTime)*Time.deltaTime);
            color.a = FadeCurve.Evaluate(timer/DropTime);
            img.color = color;
            timer += Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
