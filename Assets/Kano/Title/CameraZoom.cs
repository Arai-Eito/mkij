using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] Camera targetCamera;
    [SerializeField] float fromSize = 10f;
    [SerializeField] float toSize = 5f;
    [SerializeField] float duration = 0.6f;

    Coroutine routine;

    public void Play()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(Zoom());
    }

    IEnumerator Zoom()
    {
        float t = 0f;
        targetCamera.orthographicSize = fromSize;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            float ease = EaseOutCubic(k);

            targetCamera.orthographicSize =
                Mathf.Lerp(fromSize, toSize, ease);

            yield return null;
        }

        targetCamera.orthographicSize = toSize;
    }

    float EaseOutCubic(float x)
    {
        float a = 1f - x;
        return 1f - a * a * a;
    }
}
