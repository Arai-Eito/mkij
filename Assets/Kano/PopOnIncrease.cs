using System.Collections;
using UnityEngine;

public class PopOnIncrease : MonoBehaviour
{
    [SerializeField] float popScale = 1.12f;   // ƒ|ƒ“‚Ì‘å‚«‚³
    [SerializeField] float popInTime = 0.07f;  // Šg‘å‚É‚©‚¯‚éŽžŠÔ
    [SerializeField] float popOutTime = 0.10f; // –ß‚éŽžŠÔ

    Vector3 defaultScale;
    Coroutine co;

    void Awake()
    {
        defaultScale = transform.localScale;
    }

    public void PlayPop()
    {
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(PopRoutine());
    }

    IEnumerator PopRoutine()
    {
        // Šg‘å
        Vector3 target = defaultScale * popScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, popInTime);
            transform.localScale = Vector3.Lerp(defaultScale, target, t);
            yield return null;
        }

        // –ß‚·
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, popOutTime);
            transform.localScale = Vector3.Lerp(target, defaultScale, t);
            yield return null;
        }

        transform.localScale = defaultScale;
        co = null;
    }
}
