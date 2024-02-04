using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundLayer : MonoBehaviour
{
    Vector3 startPosition;
    Transform _transform,cameraTransform;
    [SerializeField]
    private Vector2 lerpRate;
    Vector3 lastMove;
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        cameraTransform = Camera.main.transform;
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<Callback>(EventKey.StageStart, OnGameStart);
    }
    private void OnEnable()
    {
        Messenger.AddListener<Callback>(EventKey.StageStart, OnGameStart);
    }
    private void OnGameStart(Callback cb)
    {

        startPosition = transform.localPosition;
        lastMove = cameraTransform.position;
        active = true;
    }
    bool active;
    void FixedUpdate()
    {
        if (!active) return;
        Vector3 move = cameraTransform.position - lastMove;
        move.x *= lerpRate.x;
        move.y *= lerpRate.y;
        move.z = 0;
        startPosition += move;
        _transform.localPosition = Vector3.Lerp(_transform.localPosition, startPosition, 0.3f);
        //_transform.localPosition = startPosition;

        lastMove = cameraTransform.position;
    }
}
