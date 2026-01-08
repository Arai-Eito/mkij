using UnityEngine;
using System.Collections;

public class BlockPuffer : MonoBehaviour
{

    [Header("ブロックの見た目")]
    [SerializeField] Transform _transform;
    [Header("ダメージ演出")]
    [SerializeField] float damageScale = 1.2f; //膨らむサイズ
    [SerializeField] float animTime = 0.1f; //アニメーション速度

    Vector3 defaultScale;
    Coroutine damageCoroutine;

    private void Awake()
    {
        // 初期サイズを設定
        defaultScale = _transform.localScale;
    }


    public void Play()
    {
        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);

        damageCoroutine = StartCoroutine(DamageAnim());
    }

    IEnumerator DamageAnim()
    {
        Vector3 maxScale = defaultScale * damageScale;

        // 膨らむ
        yield return ScaleLeap(_transform, defaultScale, maxScale, animTime);
        // 元に戻る
        yield return ScaleLeap(_transform, maxScale, defaultScale, animTime);

    }
    IEnumerator ScaleLeap(Transform target, Vector3 from, Vector3 to, float time)
    {
        float t = 0f;
        while (t < animTime)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(from, to, t / time);
            yield return null;
        }
        target.localScale = to;
    }
}
