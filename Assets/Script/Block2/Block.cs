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

    // 番号に関するもの
    protected int _number = 1;
    [SerializeField] TMP_Text _text;

    // 移動に関するもの
    private Vector3 _position;
    private bool _moving;


    //////////////////////////////////////////////////

    private void Start()
    {
        UpdateNumberText();
    }
    private void FixedUpdate()
    {
        // moving
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

    //////////////////////////////////////////////////

    //列で消されたとき
    public virtual void ClearLine() { }
    // 壊れたとき
    public virtual void Broken() { }
    // 移動の仕方
    public virtual void Move() { }
    // 移動の遷移
    public void MoveTo(Vector3 position) { _moving = true; _position = position; }
    // テキスト値の更新
    public void UpdateNumberText()
    {
        if (_type != BLOCK_TYPE.NONE)
        {
            _text.text = _number.ToString();
        }
    }

    //////////////////////////////////////////////////

    public BLOCK_TYPE GetBlockType() {  return _type; }
    public int2 GetIndex() {  return _index; }
    public void SetIndex(int2 index) { _index = index; }
    public int GetNumber() { return _number; }
    public void SetNumber(int number) {  _number = number; UpdateNumberText(); }
    public Vector3 GetPosition() { return _position; }

    //////////////////////////////////////////////////


}
