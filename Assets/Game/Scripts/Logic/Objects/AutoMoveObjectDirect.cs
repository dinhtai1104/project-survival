using com.mec;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Move by transform.right
/// </summary>
public class AutoMoveObjectDirect : MonoBehaviour, IMove
{
    public bool ChangeRightDirection = false;
    public Stat Speed { get; set; }
    public bool IsActive { set; get; } = true;

    private Transform _transform;
    private CoroutineHandle movingHandler;
    protected Vector3 direction = Vector3.zero;
    private Transform target;
    private float trackingRate;
    [SerializeField]
    private float trackingDelay = 0.25f, trackingDuration = 2.5f;
    [SerializeField]
    private bool unscaleTime = false;
    bool isTracking = false;
    float trackingTime = 0;
    Vector2 velocity;
    Vector3 lastTrackDirection;

    public virtual void SetMove(Vector2 move)
    {

    }
    public void Move(Stat speed)
    {
        this.Speed = speed;
        Timing.KillCoroutines(gameObject);
        movingHandler = Timing.RunCoroutine(_Moving(), gameObject);
    }

    void OnEnable()
    {
        _transform = transform;
    }
    private void OnDisable()
    {
        if (Speed != null)
        {
            Speed.ClearModifiers();
        }
        direction = Vector3.zero;
        Timing.KillCoroutines(gameObject);
        Timing.KillCoroutines(movingHandler);
    }
    public static Vector2 RotationToVector(float rotationAngle)
    {
        return new Vector2(Mathf.Cos(rotationAngle * Mathf.Deg2Rad), Mathf.Sin(rotationAngle * Mathf.Deg2Rad));
    }
    public static float AngleBetweenVectors(Vector2 from, Vector2 to)
    {
        return Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;
    }


    protected virtual IEnumerator<float> _Moving()
    {
        if (direction == Vector3.zero) direction = transform.right;
        while (true)
        {
            if (_transform == null) yield break;
            if (Speed == null || Speed.Value == 0 || IsActive == false)
            {
                yield return Time.deltaTime;
                continue;
            }
            if (target != null && isTracking)
            {
                Vector3 lookAtTargetDirection = (GetTargetPosition() - _transform.position).normalized ;

                direction = Vector3.RotateTowards(direction, lookAtTargetDirection, trackingRate*Mathf.PI,1);


                trackingTime += GameTime.Controller.FixedDeltaTime(unscaleTime);
                if (trackingTime > trackingDuration)
                {
                    isTracking = false;
                }
            }
          

            _transform.position += (direction.normalized * Speed.Value) * GameTime.Controller.FixedDeltaTime(unscaleTime);
            if (ChangeRightDirection)
                _transform.right = direction;

            //SetDirection(direction);

            yield return GameTime.Controller.FixedDeltaTime(unscaleTime);
        }
    }
    Vector3 position;
    Vector3 GetTargetPosition()
    {
        return position;
    }
    void UpdateTargetPosition()
    {
        if (target == null) return;
        position = target.position;
        if (Speed == null || Speed.Value == 0 || IsActive == false)
        {
            CancelInvoke();
        }
    }

    public void Move()
    {
        Timing.KillCoroutines(gameObject);
        movingHandler = Timing.RunCoroutine(_Moving(), gameObject);
    }

    public void Move(Stat speed, Vector2 direction)
    {
        this.direction = direction;
        this.Speed = speed;
        velocity = direction;
        Timing.KillCoroutines(gameObject);
        movingHandler = Timing.RunCoroutine(_Moving(), gameObject);
    }

    public void TrackTarget(float levelTracking, Transform target)
    {
        trackingRate = levelTracking;
        this.target = target;
        trackingTime = 0;
        isTracking = trackingRate > 0;
        InvokeRepeating(nameof(UpdateTargetPosition), 0, trackingDelay);
    }
   
    
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
        transform.right = direction;

        var localScale = transform.localScale;
        if (transform.up.y > 0)
        {
            if (localScale.y < 0)
            {
                localScale.y = -localScale.y;
            }
        }
        else
        {
            if (localScale.y > 0)
            {
                localScale.y = -localScale.y;
            }
        }
        transform.localScale = localScale;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }
    public void SetPosition(Vector3 position)
    {
    }
    public void OnUpdate()
    {
    }

    public void Stop()
    {
        Timing.KillCoroutines(gameObject);
    }
}