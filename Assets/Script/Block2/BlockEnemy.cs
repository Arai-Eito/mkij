using UnityEngine;

public class BlockEnemy : BlockMove
{
    [Header("膨らむアニメーションクラス")]
    [SerializeField] BlockPuffer puffer;

    public override void Broken()
    {
    }

    public override void Damaged()
    {
        // 膨らむアニメーション
        if (puffer != null)
            puffer.Play();
    }
}
