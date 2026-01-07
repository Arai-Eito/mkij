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

    private int _reroll = 0;


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

    public void ItemReroll()
    {
        if (_reroll <= 0) return;
        _reroll--;

        _animator.SetTrigger("Reroll");
    }
    public void SetItem()
    {
        int num = _itemlist.GetPositionMax();
        _itemlist.ItemClear();

        for(int i = 0; i < num; i++)
        {
            GameObject obj = Instantiate(_items[Random.Range(0,_items.Count)]);
            Parts p = obj.GetComponent<Parts>();
            p.SetLevel(Stage.instance.GetLevel() / 8);
            
            _itemlist.ItemSet(i,obj);


        }

    }
    public void ItemClear()
    {
        _animator.SetTrigger("ItemClear");
    }
    
    public void ItemListItemClear()
    {
        _itemlist.ItemClear(); 
    }
    public void AddReroll(int r) { _reroll += r; }
    public bool GetCanReroll() => 0 < _reroll;
}
