
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;

    [SerializeField] GameObject _item;
    
    [SerializeField] Animator _animator;
    [SerializeField] ItemList _itemlist;

    private int _reroll = 0;

    public const int MaxReroll = 3;

    


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
            GameObject obj = Instantiate(_item);
            PartsAuto p = obj.GetComponent<PartsAuto>();
            p.CreateParts(Random.Range(1,5));
            p.SetLevel(1);
            
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
    public void AddReroll(int amount)
    {
        _reroll = Mathf.Clamp(_reroll + amount, 0, MaxReroll);
    }

    public bool GetCanReroll() => 0 < _reroll;
    public int GetRerollCount() { return _reroll; }
}
