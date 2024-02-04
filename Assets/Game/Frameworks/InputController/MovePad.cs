using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhanceTouch = UnityEngine.InputSystem.EnhancedTouch;
public class MovePad : MonoBehaviour
{
    [SerializeField]
    RectTransform joystickPad, joystick,test;
    [SerializeField]
    private Vector2 touchStartPos, touchPos, dragVector, originPos;
    [SerializeField]
    private float radius, threshold;
    [SerializeField]
    Vector2 rectSize;
    [SerializeField]
    private Canvas canvas;


    public delegate void OnMove(Vector2 direction, float ratio);
    public OnMove onMove;
    System.Action<Vector2, float> onMoveChanged;
    System.Action onStop;
    public UnityEngine.InputSystem.InputAction inputAction;
    float scale;


    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectSize.Set(GetComponent<RectTransform>().rect.width, GetComponent<RectTransform>().rect.height);
        scale = canvas.scaleFactor;
        originPos = joystickPad.anchoredPosition;
        radius = joystickPad.rect.width / 3f;

    }
    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
        joystick.gameObject.SetActive(active);
        joystickPad.gameObject.SetActive(active);
    }
    public void Register(System.Action<Vector2, float> onMove, System.Action onStop)
    {
        this.onStop+= onStop;
        onMoveChanged += onMove;
    }
    public void Deregister(System.Action<Vector2, float> onMove, System.Action onStop)
    {
        this.onStop?.Invoke();
        this.onStop -= onStop;
        onMoveChanged -= onMove;
    }
    bool isActivated = false;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        EnhanceTouch.Touch.onFingerDown += Touch_onFingerDown;
        EnhanceTouch.Touch.onFingerUp += Touch_onFingerUp;
        EnhanceTouch.Touch.onFingerMove += Touch_onFingerMove;

        inputAction.Enable();
        inputAction.performed += InputAction_performed;

    }
    bool isHolding;
    private void InputAction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
    }

    private void OnDestroy()
    {
        EnhanceTouch.Touch.onFingerDown -= Touch_onFingerDown;
        EnhanceTouch.Touch.onFingerUp -= Touch_onFingerUp;
        EnhanceTouch.Touch.onFingerMove -= Touch_onFingerMove;
    }
    private void OnDisable()
    {
        moveFinger = null;
        joystickPad.anchoredPosition = originPos;
        joystick.anchoredPosition = originPos;
        onStop?.Invoke();

        EnhanceTouch.Touch.onFingerDown -= Touch_onFingerDown;
        EnhanceTouch.Touch.onFingerUp -= Touch_onFingerUp;
        EnhanceTouch.Touch.onFingerMove -= Touch_onFingerMove;
    }
    Finger moveFinger;
    Vector2 mousePosition;
    private void Touch_onFingerMove(Finger obj)
    {
        if (moveFinger != obj) return;
        if (!IsInTouchZone(obj.screenPosition)) return;

        mousePosition = obj.screenPosition / scale;
        touchPos = mousePosition - rectSize / 2f;
        dragVector = (touchPos - touchStartPos).normalized;
        float distance = Vector2.Distance(touchStartPos, touchPos);
        float ratio = Mathf.Min(distance, threshold);
        joystick.anchoredPosition = touchStartPos + dragVector * ratio;
        onMove?.Invoke(dragVector, ratio / radius);
        onMoveChanged?.Invoke(dragVector, ratio / radius);
    }

    private void Touch_onFingerUp(Finger obj)
    {
        if (moveFinger != obj) return;
        moveFinger = null;
        joystickPad.anchoredPosition = originPos;
        joystick.anchoredPosition = originPos;
        onStop?.Invoke();
    }
    private void Touch_onFingerDown(Finger obj)
    {
        if (!IsInTouchZone(obj.screenPosition)) return;
        moveFinger = obj;
        mousePosition = obj.screenPosition / scale;
        touchStartPos = touchPos = mousePosition - rectSize / 2f;
        joystickPad.anchoredPosition = touchStartPos;
        joystick.anchoredPosition = touchStartPos;


    }
    [SerializeField]
    private RectTransform touchZone;
    [SerializeField]
    private float minY=0.1f,minX=0.4f;
    bool IsInTouchZone(Vector3 touchPosition)
    {
        return touchPosition.x < Screen.width/2f &&
            touchPosition.x >  Screen.width*minX &&
            touchPosition.y < Screen.height &&
            touchPosition.y > Screen.height*minY;

    }
    Vector2 clampPos;

    private void Update()
    {
        if (!InputController.InputController.ENABLED) return;
        if (moveFinger != null)
        {
            if (Vector2.Distance(touchStartPos, touchPos) > threshold)
            {
                touchStartPos = Vector2.Lerp(touchStartPos, touchPos, 0.04f);
                //Logger.Log(touchStartPos);
                //Logger.Log(touchZone.anchoredPosition);
                //float width= touchZone.rect.width;
                //float height= touchZone.rect.height;
                //clampPos.x = Mathf.Clamp(touchStartPos.x, touchZone.anchoredPosition.x-width/2f, touchZone.anchoredPosition.x+width/2f);
                //clampPos.y = Mathf.Clamp(touchStartPos.y, touchZone.anchoredPosition.y - height / 2f, touchZone.anchoredPosition.y + height / 2f);
                joystickPad.anchoredPosition = touchStartPos;
                //test.anchoredPosition = touchStartPos;
            }
        }
        if (inputAction.phase == UnityEngine.InputSystem.InputActionPhase.Performed)
        {
            onMoveChanged?.Invoke(inputAction.ReadValue<Vector2>(),1);
        }
        else if(inputAction.WasReleasedThisFrame())
        {
            onStop?.Invoke();
        }
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            onMoveChanged?.Invoke(new Vector2(-1,0),1);
        }
        else if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            onStop?.Invoke();
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            onMoveChanged?.Invoke(new Vector2(1, 0), 1);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            onStop?.Invoke();
        }
#endif
    }
}
