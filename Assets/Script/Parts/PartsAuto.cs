using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PartsAuto : Parts
{
    [SerializeField] GameObject _normal;
    [SerializeField] GameObject _normalHit;

   

    public void CreateParts(int blockNum) 
    {
        if (blockNum <= 0) return;

        // 子どもを全部消す
        foreach(Transform child in _blockBase)
        {
            Destroy(child.gameObject);
        }

        // 位置決め
        List<Vector3> positions = new List<Vector3>();
        positions.Add(new Vector3(Random.Range(-1, 2), 0, 0));
        for(int i = 1; i < blockNum; i++)
        {
            int p = Random.Range(0, 100);
            Vector3 offset = Vector3.zero;


            float r = 1.57f * (p % 4);
            // 十字
            {
                offset += new Vector3(
                    Mathf.RoundToInt(Mathf.Sin(r)),
                    0.0f,
                    Mathf.RoundToInt(Mathf.Cos(r)));
            }
            // 斜め
            if (p < 15)
            {
                r += 1.57f * Mathf.Sign(Random.Range(-1, 1));
                offset += new Vector3(
                    Mathf.RoundToInt(Mathf.Sin(r)),
                    0.0f,
                    Mathf.RoundToInt(Mathf.Cos(r)));
            }


            Vector3 position = positions[positions.Count - 1] + offset;
            positions.Remove(position);
            positions.Add(position);
        }
        // 中央をずらす
        Vector3 center = Vector3.zero;
        for (int i = 0; i < positions.Count; i++)
        {
            center += positions[i];
        }
        center /= positions.Count;
        for(int i = 0; i < positions.Count;i++)
        {
            positions[i] -= center;
        }

        // ブロックを置く
        _blocks = new Block[positions.Count];
        bool normalhit = Random.Range(0, 2) == 0;
        for (int i = 0; i < positions.Count; i ++)
        {
            // 属性切り替え
            int p = Random.Range(0, 100);
            if(p < 20)
            {
                normalhit = !normalhit;
            }

            //ブロックを生成
            GameObject obj = Instantiate(
                (normalhit) ? _normalHit : _normal,
                _blockBase);
            obj.transform.position = transform.position + positions[i];
            _blocks[i] = obj.GetComponent<Block>();

        }
    }
}
