using UnityEngine;
using System.Collections;

public class IconReaction : MonoBehaviour
{
    [Header("Add (gain)")]
    [SerializeField] float addScale = 1.3f;
    [SerializeField] float addTime = 0.08f;

    [Header("Use (consume)")]
    [SerializeField] float useScale = 0.85f;
    [SerializeField] float useTime = 0.08f;

    [Header("Use Shake (optional)")]
    [SerializeField] bool useShake = true;
    [SerializeField] float shakeTime = 0.10f;
    [SerializeField] float shakeStrength = 6f; // ローカル位置の揺れ幅

    Vector3 defaultScale;
    Vector3 defaultLocalPos;
    Coroutine co;

    void Awake()
    {
        defaultScale = transform.localScale;
        defaultLocalPos = transform.localPosition;
    }

    public void PlayAdd()
    {
        StartOne(AddRoutine());
    }

    public void PlayUse()
    {
        if (useShake)
            StartOne(UseShakeRoutine());
        else
            StartOne(UseShrinkRoutine());
    }

    void StartOne(IEnumerator routine)
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(routine);
    }

    IEnumerator AddRoutine()
    {
        transform.localScale = defaultScale * addScale;
        yield return new WaitForSeconds(addTime);
        transform.localScale = defaultScale;
    }

    IEnumerator UseShrinkRoutine()
    {
        transform.localScale = defaultScale * useScale;
        yield return new WaitForSeconds(useTime);
        transform.localScale = defaultScale;
    }

    IEnumerator UseShakeRoutine()
    {
        // シュッと縮め
        transform.localScale = defaultScale * useScale;

        float t = 0f;
        while (t < shakeTime)
        {
            t += Time.deltaTime;

            // ランダム揺れ（毎フレーム）
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;
            transform.localPosition = defaultLocalPos + new Vector3(x, y, 0f);

            yield return null;
        }

        // 後始末
        transform.localPosition = defaultLocalPos;
        transform.localScale = defaultScale;
    }

    void OnDisable()
    {
        // 途中で非表示になってもズレを残さない
        if (co != null) StopCoroutine(co);
        transform.localPosition = defaultLocalPos;
        transform.localScale = defaultScale;
        co = null;
    }
}
