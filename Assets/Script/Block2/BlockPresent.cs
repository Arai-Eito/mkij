using UnityEngine;

public class BlockPresent : BlockMove
{
    public override void Broken()
    {
        ItemManager.instance.AddReroll(1);
        ScoreManager.instance.AddScore(1);
    }
}
