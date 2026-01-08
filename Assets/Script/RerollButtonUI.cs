using UnityEngine;
using UnityEngine.UI;

public class RerollButtonUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private ButtonAnim buttonAnim;
    [SerializeField] private PopOnIncrease pop;
    private int _prevCount = -999;

    [Header("Sprites / Textures")]
    [SerializeField] private Sprite spriteDisabled; // 灰色
    [SerializeField] private Sprite spriteEnabled;  // 押せる

    [Header("Count Text (optional)")]
    [SerializeField] private GameObject countRoot;          // テキストの親(表示/非表示)
    [SerializeField] private Text countText;
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

    // Button の OnClick に登録する
    public void OnClickReroll()
    {
        if (!ItemManager.instance) return;
        if (!ItemManager.instance.GetCanReroll()) return;

        ItemManager.instance.ItemReroll();

        // 押した直後にUI更新
        Refresh();
    }
    private void Refresh()
    {
        if (!ItemManager.instance) return;

        int count = ItemManager.instance.GetRerollCount();
        bool can = count > 0;

        // 押せるか
        if (button)
            button.interactable = can;

        // 見た目
        if (buttonImage)
            buttonImage.sprite = can ? spriteEnabled : spriteDisabled;

        // 縮小アニメのON/OFF
        if (buttonAnim)
            buttonAnim.enabled = can;

        // 回数表示
        if (countRoot)
            countRoot.SetActive(can);

        if (countText)
            countText.text = count.ToString(); // "x"付けたければ $"x{count}"

        // 初回だけ膨らまない
        if (_prevCount == -999)
        {
            _prevCount = count;
        }
        else
        {
            // 増えた瞬間に膨らむ
            if (count > _prevCount)
            {
                if (pop) pop.PlayPop();
            }
            _prevCount = count;
        }
    }
}