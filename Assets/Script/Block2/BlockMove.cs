using Unity.Mathematics;
using UnityEngine;

public class BlockMove : Block
{


    public override void Move()
    {
        int2 toindex = _index + new int2(0, -1); 

        if(Stage.instance.CheckIndex(toindex))
        {
            Block toblock = Stage.instance.GetBlock(toindex);

            switch (toblock.GetBlockType())
            {
                case BLOCK_TYPE.NONE:
                case BLOCK_TYPE.MOVE:
                    Stage.instance.SwapBlock(_index, toindex);
                    break;

                case BLOCK_TYPE.NOMOVE:

                    int number = toblock.GetNumber() - _number;
                    if(number == 0)
                    {
                        Stage.instance.DeleteBlock(toindex);
                        Stage.instance.DeleteBlock(_index);
                    }
                    else if(number < 0)
                    {
                        SetNumber(-number);
                        Stage.instance.DeleteBlock(toindex);

                        Stage.instance.SwapBlock(_index, toindex);
                    }
                    else
                    {
                        toblock.SetNumber(number);
                        Stage.instance.DeleteBlock(_index);
                    }

                    break;
            }


        }
        else
        {
            Taiho taiho = Stage.instance.GetTaiho();

            int number = taiho.GetNumber() - _number;
            if(number < 0) number = 0;
            taiho.SetNumber(number);
            taiho.UpdateNumberText();

            // Ž©•ªŽ©g‚ðÁ‚·
            Stage.instance.DeleteBlock(_index);
        }
    }

}
