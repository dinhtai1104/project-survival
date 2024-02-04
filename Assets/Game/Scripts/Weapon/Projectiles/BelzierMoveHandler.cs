using UnityEngine;

public class BelzierMoveHandler:MonoBehaviour
{
    [SerializeField]
    private float MiddlePointOffset=1;
    private float speed = 1;
    Transform _transform,target;
    Vector3 startPoint,middlePoint;
    System.Action onArrived;
    [SerializeField]
    private AnimationCurve moveCurve;
    private void OnEnable()
    {
        target = null;
    }
    private void OnDisable()
    {
        target = null;
    }

    public void Move(Transform target,System.Action onArrived)
    {
        this.onArrived = onArrived;
        _transform = transform;
        this.target = target;
        startPoint = _transform.position;

        float distance = Vector2.Distance(target.position, startPoint);
        Vector3 rd = (Vector3)Random.insideUnitCircle * Random.Range(distance * 0.1f*MiddlePointOffset / 2f, distance * 0.2f * MiddlePointOffset / 2f);
        middlePoint = startPoint + (target.position - startPoint) / 2f + rd;
        t = 0;
        maxTime = moveCurve.keys[moveCurve.length - 1].time;
        speed = UnityEngine.Random.Range(0.8f, 1.1f);
    }

    float t = 0;
    float maxTime = 0;
    float a = 0;
    private void Update()
    {
        if (target == null) return;
        if (t < 1)
        {
            _transform.localPosition = CalculatePoint(moveCurve.Evaluate(t*maxTime), startPoint, middlePoint, target.position);
            t += Time.deltaTime*speed  ;
         
        }
        else
        {
            onArrived?.Invoke();
        }
    }
    public Vector2 CalculatePoint(float t, Vector2 a, Vector2 b, Vector2 o)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector2 p = uu * a;
        p += 2 * u * t * b;
        p += tt * o;
        return p;
    }
}