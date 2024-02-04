using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETouch=UnityEngine.InputSystem.EnhancedTouch;
public class TouchPointer : MonoBehaviour
{
    RectTransform _transform,parentRect;
    Camera cam;
    Vector2 position;
    [SerializeField]
    private ParticleSystem ps;
    private void Start()
    {
        _transform = GetComponent<RectTransform>();
        parentRect = _transform.parent.GetComponent<RectTransform>();
        cam = Camera.main;
    }
    private void OnEnable()
    {
        ETouch.Touch.onFingerDown += Touch_onFingerDown;
    }
    private void OnDestroy()
    {
        ETouch.Touch.onFingerDown -= Touch_onFingerDown;
    }
    private void OnDisable()
    {
        ETouch.Touch.onFingerDown -= Touch_onFingerDown;
    }

    private void Touch_onFingerDown(ETouch.Finger obj)
    {
        if (GameUIPanel.Instance != null && GameUIPanel.Instance.gameObject.activeSelf) return;
        if (Sound.Controller.Instance != null)
            Sound.Controller.Instance.PlayClickSFX();
        ps.Play();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, obj.screenPosition, cam, out position);
        _transform.anchoredPosition = position;
    }

}
