using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineClearOrbAnimator : MonoBehaviour
{
    [Serializable]
    public struct OrbItem
    {
        public Vector3 startPos;
        public int value;

        public OrbItem(Vector3 p, int v)
        {
            startPos = p;
            value = v;
        }
    }

    [SerializeField] private GameObject orbPrefab;

    [Header("Motion")]
    [SerializeField] private float travelTime = 0.35f;
    [SerializeField] private float spawnInterval = 0.06f;
    [SerializeField] private float arcHeight = 2.0f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private Transform orbParent;

    /// <summary>
    /// オーブが到着するたびに onArriveValue(value) を呼ぶ。
    /// 全部終わったら onAllComplete() を呼ぶ（不要ならnullでOK）。
    /// </summary>
    public void Play(
        IList<OrbItem> items,
        Transform target,
        Action<int> onArriveValue,
        Action onAllComplete = null)
    {
        if (orbPrefab == null || target == null || items == null || items.Count == 0)
        {
            onAllComplete?.Invoke();
            return;
        }

        StartCoroutine(PlayCo(items, target, onArriveValue, onAllComplete));
    }

    private IEnumerator PlayCo(
        IList<OrbItem> items,
        Transform target,
        Action<int> onArriveValue,
        Action onAllComplete)
    {
        int alive = 0;

        for (int i = 0; i < items.Count; i++)
        {
            alive++;
            OrbItem item = items[i];
            SpawnAndFly(item, target, onArriveValue, () => alive--);

            yield return new WaitForSeconds(spawnInterval);
        }

        while (alive > 0)
            yield return null;

        onAllComplete?.Invoke();
    }

    private void SpawnAndFly(
        OrbItem item,
        Transform target,
        Action<int> onArriveValue,
        Action onArriveDone)
    {
        Transform parent = orbParent != null ? orbParent : transform;
        GameObject orb = Instantiate(orbPrefab, item.startPos, Quaternion.identity, parent);

        StartCoroutine(FlyBezierCo(orb.transform, item.startPos, target, () =>
        {
            // ★ 到着した瞬間に加算
            onArriveValue?.Invoke(item.value);

            // 消す/SEなど
            if (orb != null) Destroy(orb);

            onArriveDone?.Invoke();
        }));
    }

    private IEnumerator FlyBezierCo(Transform orb, Vector3 start, Transform target, Action onArrive)
    {
        Vector3 end = target.position;
        Vector3 mid = (start + end) * 0.5f;
        Vector3 control = mid + Vector3.up * arcHeight;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, travelTime);
            float et = ease != null ? ease.Evaluate(Mathf.Clamp01(t)) : Mathf.Clamp01(t);

            Vector3 p = QuadraticBezier(start, control, end, et);
            if (orb != null) orb.position = p;

            yield return null;
        }

        onArrive?.Invoke();
    }

    private static Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1f - t;
        return (u * u) * a + (2f * u * t) * b + (t * t) * c;
    }
}
