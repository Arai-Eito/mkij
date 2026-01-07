using TMPro;
using UnityEngine;


public enum BlockType 
{
    None = 0,
    Normal,
    Special,
    Item,
    Boss,
    Enemy,
};


public class Block2 : MonoBehaviour
{
    [SerializeField] BlockType _type;
    [SerializeField] protected int _moveLength;
    public TMP_Text _text;

    public int _number;

    private bool _moving = false;
    private Vector3 _position;


    ////////////////////////////////////////

    private void Start()
    {
        UpdateNumberText();
    }
    private void FixedUpdate()
    {
        if (!_moving) return;

        Vector3 direction = _position - transform.position;
        float length = direction.magnitude;
        direction = direction.normalized;

        float speed = 12 * Time.deltaTime;
        if(length < speed)
        {
            transform.position = _position;
        }
        else
        {
            transform.position += direction * speed;
        }
    }



    public void UpdateNumberText()
    {
        if (_type != BlockType.None)
        {
            _text.text = _number.ToString();
        }
    }
    public void Move(Vector3 position)
    {
        _moving = true;

        _position = position;
    }
    public Vector3 GetPosition() { return _position; } 
    public BlockType GetBlockType() { return _type; }
    public int GetNumber() { return _number; }
    public void SetNumber(int number) { _number = number; }
    public int GetMoveLenghth() { return _moveLength; }
}
