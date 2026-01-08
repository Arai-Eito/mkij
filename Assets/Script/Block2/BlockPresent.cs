using System.Collections;
using UnityEngine;

public class BlockPresent : BlockMove
{
    [Header("膨らむアニメーションクラス")]
    [SerializeField] BlockPuffer puffer;

    public override void Broken()
    {
        ItemManager.instance.AddReroll(1);
    }
    
    public override void Damaged()
    {
        // 膨らむアニメーション
        if (puffer != null)
            puffer.Play();
    }
}
