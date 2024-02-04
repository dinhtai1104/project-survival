using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetAim : MonoBehaviour
{
    public static TargetAim Instance;
    RectTransform _transform, parentRect;
    Camera cam;
    Vector2 position;
    [SerializeField]
    private Transform pointer;
    Animator anim;
    bool active = false;
    private void Start()
    {
        Instance = this;

        anim=GetComponent<Animator>();
        _transform = GetComponent<RectTransform>();
        parentRect = _transform.parent.GetComponent<RectTransform>();
        cam = Camera.main;
        GameController.onStageStart -= SetUp;
        GameController.onStageStart += SetUp;

        GameController.onStageEnd -= Clear;
        GameController.onStageEnd += Clear;
        Clear();
    }
    private void OnDestroy()
    {
        GameController.onStageStart -= SetUp;
        GameController.onStageEnd -= Clear;
    }
    public void SetUp()
    {
        active = true;
    }
    public void Clear()
    {
        active = false;
        pointer.gameObject.SetActive(false);
    }
    bool isHolding = false;
    public void Hold()
    {
        if (!active)
        {
            return;
        }
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, touch.position, cam, out position);
            _transform.anchoredPosition = position;
        }
        else
        {
        }
    }

    public void Trigger()
    {
        if (!active) return;
        pointer.gameObject.SetActive(true);
        isHolding = true;
        anim.SetTrigger("Hold");
    }
    public void Release()
    {
        if (!active) return;
        isHolding = false;
        anim.SetTrigger("Release");
    }


}
