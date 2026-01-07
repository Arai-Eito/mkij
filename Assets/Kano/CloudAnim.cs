using UnityEngine;
using System.Collections;

public class CloudAnim : MonoBehaviour
{
    public float duration = 0.4f;
    public float scaleTo = 2.2f;
    public float moveX = 8f;
    public float moveZ = -6f;

    Vector3 startPos;
    Vector3 startScale;

    void Awake()
    {
        startPos = transform.position;
        startScale = transform.localScale;
    }

    public IEnumerator Play(bool moveRight)
    {
        float t = 0f;

        Vector3 endPos = startPos;
        endPos.x += moveRight ? moveX : -moveX;
        endPos.z += moveZ;

        Vector3 endScale = startScale * scaleTo;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / duration);
            float ease = EaseInCubic(k);

            transform.position = Vector3.Lerp(startPos, endPos, ease);
            transform.localScale = Vector3.Lerp(startScale, endScale, ease);

            yield return null;
        }

        gameObject.SetActive(false);
    }

    float EaseInCubic(float x) => x * x * x;
}
