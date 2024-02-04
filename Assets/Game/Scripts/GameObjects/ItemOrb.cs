using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOrb : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem ps;
    Transform target;
    Transform _transform;
    Vector3 middlePoint;
    Vector3 startPoint;
    System.Action<ItemOrb> onArrived;
    [SerializeField]
    private AudioClip sfx;
    float speed = 1;
    [SerializeField]
    private SpriteRenderer sr;
    public void SetUp(Transform from, Transform target, float speed, System.Action<ItemOrb> onArrived)
    {
        if (_transform == null)
            _transform = transform;
        this.onArrived = onArrived;
        this.speed = speed;
        gameObject.SetActive(true);
        if(ps!=null)
        ps.Play();
        _transform.position = from.position;
        this.startPoint = from.position;
        float distance = Vector2.Distance(target.position, from.position);
        Vector3 rd = (Vector3)Random.insideUnitCircle * Random.Range(distance * 0.1f / 2f, distance * 0.2f / 2f);
        middlePoint = from.position + (target.position - from.position) / 2f + rd;
        this.target = target;
        t = 0;
        a = 0.1f;
    }
    public void SetUp(Vector3 fromPos, Transform target, float speed, System.Action<ItemOrb> onArrived)
    {
        if (_transform == null)
            _transform = transform;
        this.onArrived = onArrived;
        this.speed = speed;
        gameObject.SetActive(true);
        if(ps!=null)
            ps.Play();
        _transform.position = fromPos;
        this.startPoint = fromPos;
        float distance = Vector2.Distance(target.position, fromPos);
        Vector3 rd = (Vector3)Random.insideUnitCircle * Random.Range(distance * 0.1f / 2f, distance * 0.2f / 2f);
        middlePoint = fromPos + (target.position - fromPos) / 2f + rd;
        this.target = target;
        t = 0;
        a = 0.1f;
    }
    float t = 0;
    float a = 0;
    private void Update()
    {
        if (target == null) return;
        if (t < 1)
        {
            _transform.localPosition = CalculatePoint(t, startPoint, middlePoint, target.position);
            t += Time.deltaTime * a * a * speed;
            a += Time.deltaTime;
            a = a < 4f ? a : 4f;
        }
        else
        {
            onArrived?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
    public Vector3 CalculatePoint(float t, Vector3 a, Vector3 b, Vector3 o)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * a;
        p += 2 * u * t * b;
        p += tt * o;
        return p;
    }
}
