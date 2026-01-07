using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CloudAnim : MonoBehaviour
{
    [Header("順番に動かす雲（UI ImageのRectTransform）を入れる")]
    [SerializeField] private RectTransform[] clouds;

    [Header("演出パラメータ")]
    [SerializeField] private float interval = 0.08f;     // 次の雲を開始するまでの間隔
    [SerializeField] private float duration = 0.35f;     // 1枚の雲が消えるまでの時間
    [SerializeField] private float scaleTo = 2.2f;       // どれだけ拡大するか（近づく感じ）
    [SerializeField] private float moveOutX = 1400f;     // どれだけ横に飛ばすか（画面外まで）
    [SerializeField] private bool alternateLR = true;    // 左右交互に飛ばす

    [Header("完了時に呼ぶ（シーン遷移など）")]
    public UnityEvent onFinished;

    private Vector3[] defaultScales;
    private Vector2[] defaultPos;

    void Awake()
    {
        defaultScales = new Vector3[clouds.Length];
        defaultPos = new Vector2[clouds.Length];
        for (int i = 0; i < clouds.Length; i++)
        {
            defaultScales[i] = clouds[i].localScale;
            defaultPos[i] = clouds[i].anchoredPosition;
        }
    }

    public void Play()
    {
        // 連打対策：必要ならボタン側でInteractable=falseにしてね
        StopAllCoroutines();
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        // 念のため初期化（再生前に戻す）
        for (int i = 0; i < clouds.Length; i++)
        {
            if (!clouds[i]) continue;
            clouds[i].gameObject.SetActive(true);
            clouds[i].localScale = defaultScales[i];
            clouds[i].anchoredPosition = defaultPos[i];
        }

        for (int i = 0; i < clouds.Length; i++)
        {
            if (!clouds[i]) continue;
            StartCoroutine(AnimateOne(clouds[i], i));
            yield return new WaitForSeconds(interval);
        }

        // 全員終わるまで待つ（最後の開始 + duration）
        yield return new WaitForSeconds(duration + 0.05f);

        onFinished?.Invoke();
    }

    private IEnumerator AnimateOne(RectTransform rt, int index)
    {
        float t = 0f;

        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos;

        float dir = 1f;
        if (alternateLR) dir = (index % 2 == 0) ? -1f : 1f; // 左右交互
        endPos.x += moveOutX * dir;

        Vector3 startScale = rt.localScale;
        Vector3 endScale = startScale * scaleTo;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // （Time.timeScale=0でも動く）
            float k = Mathf.Clamp01(t / duration);

            // イージング（最初ゆっくり→最後速い）
            float ease = EaseInCubic(k);

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, ease);
            rt.localScale = Vector3.Lerp(startScale, endScale, ease);

            yield return null;
        }

        rt.anchoredPosition = endPos;
        rt.localScale = endScale;

        // 消す
        rt.gameObject.SetActive(false);
    }

    private float EaseInCubic(float x) => x * x * x;
}
