using UnityEngine;

public class BlockEnemy : BlockMove
{
    public override void Broken()
    {
        ScoreManager.instance.AddScore(1);
    }
}
