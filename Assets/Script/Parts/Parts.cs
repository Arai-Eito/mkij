using NUnit.Framework;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Parts : MonoBehaviour
{
    public Block[] _blocks;
    [SerializeField] Vector3 _setIndexOffset;

    [Header("持ったときのパラメータ")]
    [SerializeField] protected Transform _blockBase;
    [SerializeField] float _havingSize, _noHavingSize;
    protected bool _isHaving = false;


    public void SetPrediction()
    {
        Stage stage = Stage.instance;

        for(int i = 0; i < _blocks.Length; i ++)
        {
            Vector3 pos = _blocks[i].transform.position + _setIndexOffset;
            int2 index = stage.GetIndex(pos);

            if(stage.CheckIndex(index) == true)
            {
                pos = stage.GetBlock(index).GetPosition();

                SetPartsPrediction.instance.SetPredictionBlock(i, pos, stage.CheckSetBlock(index));
            }
            else
            {
                SetPartsPrediction.instance.InvisiblePredictionBlock(i);
            }
        }
    }
    public bool SetBlock(Stage stage)
    {

        bool success = true;
        for(int i = 0; i < _blocks.Length; i++)
        {
            Vector3 pos = _blocks[i].transform.position + _setIndexOffset;
            int2 index = stage.GetIndex(pos);


            // 予測を消す
            SetPartsPrediction.instance.InvisiblePredictionBlock(i);


            // 一つでも置けなければさいなら
            if (stage.CheckSetBlock(index) == false) success = false ;
        }
        if (success == false) return false;


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

    public void SetHaving(bool b)
    {
        _isHaving = b;
        StartCoroutine(SetBaseSize(_isHaving ? _havingSize : _noHavingSize));
    }
        
    IEnumerator SetBaseSize(float size)
    {
        bool having = _isHaving;
        float currentSize = _blockBase.localScale.x ;

        for(int i = 0; i < 100;i++)
        {
            currentSize = Mathf.Lerp(currentSize, size, 0.1f);
            _blockBase.localScale = new Vector3(currentSize, currentSize, currentSize);

            if(having != _isHaving) yield break;

            yield return null;
        }

        _blockBase.localScale = new Vector3(size, size, size);

        yield break;
    }
}
