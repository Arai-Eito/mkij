using UnityEngine;
using System.Collections;

public class CanvasGroupFade : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float duration = 0.4f;

    Coroutine routine;

    public void FadeOut()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;

        canvasGroup.alpha = from;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            float ease = EaseOutCubic(k);

            canvasGroup.alpha = Mathf.Lerp(from, to, ease);
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    float EaseOutCubic(float x)
    {
        float a = 1f - x;
        return 1f - a * a * a;
    }
}
