using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class Parts : MonoBehaviour
{
    public Block[] _blocks;
    [SerializeField] Vector3 _setIndexOffset;

    public bool SetBlock(Stage stage)
    {

        for(int i = 0; i < _blocks.Length; i++)
        {
            Vector3 pos = _blocks[i].transform.position;
            int2 index = stage.GetIndex(pos);
            
            // 一つでも置けなければさいなら
            if (stage.CheckSetBlock(index) == false) return false;
        }

        // ブロックを設置する
        for(int i = 0; i < _blocks.Length; i++)
        {
            Vector3 pos = _blocks[i].transform.position + _setIndexOffset;
            int2 index = stage.GetIndex(pos);
            _blocks[i].SetIndex(index);


            stage.SetBlock(index, _blocks[i]);
        }

        return true;
    }


    public void SetLevel(int level)
    {
        for(int i = 0; i < _blocks.Length;i++)
        {
            int l = level;
            l += UnityEngine.Random.Range(0, 5);

            _blocks[i].SetNumber(l);

        }

    }
}
