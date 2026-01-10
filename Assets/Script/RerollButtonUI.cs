using UnityEngine;
using UnityEngine.UI;

public class RerollButtonUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private ButtonAnim buttonAnim;
    [SerializeField] private PopOnIncrease pop;

    [Header("Sprites / Textures")]
    [SerializeField] private Sprite spriteDisabled; // 灰色
    [SerializeField] private Sprite spriteEnabled;  // 押せる

    [Header("Reroll Icons (0..3)")]
    [SerializeField] private Image[] rerollIcons;   // プレゼント箱3つ（左→右）
    [SerializeField] private PopOnIncrease[] iconPops;     // 同じ順番で3つ
    [SerializeField] private Sprite iconOff;         // 灰
    [SerializeField] private Sprite iconOn;          // 白
    [SerializeField] private int maxReroll = 3;

    [Header("Count Text (optional)")]
    [SerializeField] private GameObject countRoot;
    [SerializeField] private Text countText;

    private int _prevCount = -999;

    private void Reset()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        buttonAnim = GetComponent<ButtonAnim>();
        pop = GetComponent<PopOnIncrease>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    public void OnClickReroll()
    {
        if (!ItemManager.instance) return;

        // クリック時点でUI側も上限＆下限を保証（保険）
        int count = Mathf.Clamp(ItemManager.instance.GetRerollCount(), 0, maxReroll);
        if (count <= 0) return;

        ItemManager.instance.ItemReroll();
        Refresh();
    }

    private void Refresh()
    {
        if (!ItemManager.instance) return;

        int raw = ItemManager.instance.GetRerollCount();
        int count = Mathf.Clamp(raw, 0, maxReroll);
        bool can = count > 0;

        if (button) button.interactable = can;
        if (buttonImage) buttonImage.sprite = can ? spriteEnabled : spriteDisabled;
        if (buttonAnim) buttonAnim.enabled = can;

        RefreshIcons(count);

        // 初回は何もしない
        if (_prevCount == -999)
        {
            _prevCount = count;
            return;
        }

        // 増えたとき
        if (count > _prevCount)
        {
            int index = count - 1; // 新しく点いた箱
            if (index >= 0 && index < iconPops.Length)
                iconPops[index]?.PlayPop();
        }
        // 使ったとき
        else if (count < _prevCount)
        {
            int index = _prevCount - 1; // 消えた箱
            if (index >= 0 && index < iconPops.Length)
                iconPops[index]?.PlayPop();
        }

        _prevCount = count;
    }

    private void RefreshIcons(int count)
    {
        for (int i = 0; i < rerollIcons.Length; i++)
        {
            bool on = i < count;
            rerollIcons[i].sprite = on ? iconOn : iconOff;
        }
    }

}
