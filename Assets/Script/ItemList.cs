using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemList : MonoBehaviour
{
    [SerializeField] List<Transform> _itemposition;

    private List<GameObject> _item = new List<GameObject>();

    //////////////////////////////////////////////////



    //////////////////////////////////////////////////

    public void ItemClear()
    {
        for (int i = 0; i < _item.Count; i++)
        {
            if (_item[i] == null) continue;

            Destroy(_item[i]);
        }
    }
    public void ItemSet(int positionindex,GameObject obj)
    {
        if (positionindex < 0 || _itemposition.Count <= positionindex) return;

        obj.transform.SetParent(_itemposition[positionindex]);
        obj.transform.position = _itemposition[positionindex].position;

        _item.Add(obj);
    }

    //////////////////////////////////////////////////

    public int GetPositionMax() => _itemposition.Count;
}
