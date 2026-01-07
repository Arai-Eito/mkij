using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonAnim : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] float pressScale = 0.9f;
    [SerializeField] float speed = 15f;

    Vector3 defaultScale;
    Coroutine scaleCoroutine;

    void Awake()
    {
        defaultScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartScale(defaultScale * pressScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartScale(defaultScale);
    }

    void StartScale(Vector3 target)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleRoutine(target));
    }

    IEnumerator ScaleRoutine(Vector3 target)
    {
        while (Vector3.Distance(transform.localScale, target) > 0.001f)
        {
            transform.localScale =
                Vector3.Lerp(transform.localScale, target, Time.deltaTime * speed);
            yield return null;
        }
        transform.localScale = target;
    }
}
