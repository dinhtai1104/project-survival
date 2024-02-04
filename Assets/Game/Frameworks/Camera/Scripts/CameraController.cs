using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    Cinemachine.CinemachineVirtualCamera cam;
    Cinemachine.CinemachineBasicMultiChannelPerlin noise;
    Transform _transform;

    public float cameraLockHalfWidth = 10;

    [SerializeField]
    private float maxHeightThreshold=3,followSpeed=0.2f;
    [SerializeField]
    private Vector3 offset;
    private Vector3 manualOffset;

    float defaultLenSize;
    float power, duration;
    bool active = false;
    float time = 0;
    bool useThreshold = true;
    Transform target;

    private void Start()
    {
        Instance = this;
        cam = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>();
        _transform = cam.transform;
        noise = cam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        SetCameraSize(cameraLockHalfWidth);
        defaultLenSize = cam.m_Lens.OrthographicSize;

    }
   
    public void ResetCameraSize()
    {
        SetCameraSize(cameraLockHalfWidth);
    }
    public void SetCameraSize(float size)
    {
        float ratio = Screen.width*1f / Screen.height;
        float realWidth = size / ratio;

        SetSize(realWidth);
        defaultLenSize = cam.m_Lens.OrthographicSize;
    }


    public void SetSize(float size)
    {
        cam.m_Lens.OrthographicSize = size;
    }

    public void SetBoundary(Collider2D collider)
    {
        cam.GetComponent<Cinemachine.CinemachineConfiner>().m_BoundingShape2D=collider;
    }

  
    public void Follow(Transform target, Vector2 offset, bool useThreshold=true)
    {
        this.target = target;
        this.useThreshold = useThreshold;
        manualOffset = offset;

    }
 
    Vector3 newPos = Vector3.zero;
    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPos = target.position;
            if ( useThreshold&&targetPos.y < maxHeightThreshold)
            {
                targetPos.y = 0;
            }
            //newPos = Vector3.Lerp(_transform.localPosition, targetPos + offset, followSpeed);
            newPos.x = Mathf.Lerp(_transform.localPosition.x, targetPos.x + offset.x+manualOffset.x, followSpeed);
            newPos.y = Mathf.Lerp(_transform.localPosition.y, Mathf.Max(0,(targetPos.y-(useThreshold?maxHeightThreshold:0))) + offset.y+manualOffset.y, 0.05f);
            newPos.z = offset.z;
            _transform.localPosition =newPos;
            
        }
      
    }
    public Transform GetTransform()
    {
        return _transform;
    }
}
