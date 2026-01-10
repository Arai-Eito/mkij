using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public enum BLOCK_TYPE
{
    NONE = 0,
    MOVE,
    NOMOVE,
};



public class Block : MonoBehaviour
{
    [SerializeField] BLOCK_TYPE _type;
    protected int2 _index;

    // î‘çÜÇ…ä÷Ç∑ÇÈÇ‡ÇÃ
    protected int _number = 1;
    [SerializeField] TMP_Text _text;

    // à⁄ìÆÇ…ä÷Ç∑ÇÈÇ‡ÇÃ
    private Vector3 _position;
    private bool _moving;


    private void Start()
    {
        UpdateNumberText();
    }
    private void FixedUpdate()
    {
        if(_moving)
        {
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
    }
    //óÒÇ≈è¡Ç≥ÇÍÇΩÇ∆Ç´
    public virtual void ClearLine() { }
    public virtual void Damaged() { }
    // âÛÇÍÇΩÇ∆Ç´
    public virtual void Broken() { }
    // à⁄ìÆÇÃédï˚
    public virtual void Move() { }
    public void MoveTo(Vector3 position) { _moving = true; _position = position; }
    public void UpdateNumberText()
    {
        if (_type != BLOCK_TYPE.NONE)
        {
            _text.text = _number.ToString();
        }
    }

    public BLOCK_TYPE GetBlockType() {  return _type; }
    public int2 GetIndex() {  return _index; }
    public void SetIndex(int2 index) { _index = index; }
    public int GetNumber() { return _number; }
    public void SetNumber(int number) {  _number = number; UpdateNumberText(); }
    public Vector3 GetPosition() { return _position; }

}
