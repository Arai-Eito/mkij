using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [SerializeField] List<GameObject> _items;
    


    [SerializeField] Animator _animator;
    [SerializeField] ItemList _itemlist;

    private bool _in = false;
    private int _reroll = 0;



    //////////////////////////////////////////////////

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    //////////////////////////////////////////////////

    public void ItemListIn()
    {
        _in = true;
        _animator.SetBool("In", _in);
    }
    public void ItemListOut()
    {
        _in = false;
        _animator.SetBool("In", _in);
    }
    public void SetItem()
    {
        if (_reroll <= 0) return;


        int num = _itemlist.GetPositionMax();
        _itemlist.ItemClear();

        for(int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(_items[Random.Range(0,_items.Count)]);
            _itemlist.ItemSet(i,obj);
        }

        // 
        ItemListIn();
    }
    public void AddReroll(int r) { _reroll += r; }

}
