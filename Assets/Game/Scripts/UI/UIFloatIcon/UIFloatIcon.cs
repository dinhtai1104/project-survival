using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIFloatIcon : MonoBehaviour
{
    public AnimationCurve moveCurve;
    public FloatIconAnimationCurveCollection collection;
    public float delay;
    public Image icon;
    public float height = 2;
    public float speed = 2;

    private Vector3 from;
    private Vector3 to;
    private float duration;
    private GameObject target;
    private System.Action<GameObject> callback;
    private AnimationCurve curve;
    public void Set(Sprite icon, Vector3 from, RectTransform target, float time = 0.5f, System.Action<GameObject> callback = null)
    {
        this.icon.sprite = icon;
        (transform as RectTransform).position = from;
        this.duration = time;
        this.from = from;
        this.target = target.gameObject;
        to = target.position;
        curve = collection.moveOffsetCurve.Random();
        this.callback = callback;
    }

    public async UniTask RunTmp()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        float time = 0;
        while(time < duration)
        {
            var targetPos = Vector3.Lerp(from, to, moveCurve.Evaluate(time / duration));
            var offsetY = curve.Evaluate((time / duration) * 100);
            targetPos += new Vector3(0, offsetY) / 100f;
            targetPos.z = 0;
            (transform as RectTransform).position = targetPos;
            time += Time.deltaTime;

            await UniTask.Delay(System.TimeSpan.FromSeconds(Time.deltaTime));
        }
        callback?.Invoke(target);
        target.transform.DOScale(Vector3.one * 0.7f, 0.1f).SetEase(Ease.InSine).OnComplete(() =>
        {
            target.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnKill(() =>
            {
                target.transform.localScale = Vector3.one;
            });
        }).OnKill(() =>
        {
            target.transform.localScale = Vector3.one;
        }).ToUniTask().Forget();
        PoolManager.Instance.Despawn(gameObject);
    }

    bool isRun = false;
    private Vector3 mid;
    public async UniTask Run()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay));
        time = 0;

        mid = (from + to) * 0.5f;
        var distance = GameUtility.GameUtility.GetRange(from, to);
        Vector3 rd = (Vector3)Random.insideUnitCircle * Random.Range(distance * 0.1f * height / 2f, distance * 0.2f / 2f);
        mid = from + (to - from) / 2f + rd;
        isRun = true;

        await UniTask.WaitUntil(() => !isRun);
        callback?.Invoke(target);
        
        PoolManager.Instance.Despawn(gameObject);
    }
    float time = 0;

    private void Update()
    {
        if (isRun == false) return;

        if (time < duration)
        {
            var targetPos = CalculatePoint(moveCurve.Evaluate(time / duration), from, mid, to);
            (transform as RectTransform).position = targetPos;
            time += Time.deltaTime * speed;
        }
        else
        {
            isRun = false;
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