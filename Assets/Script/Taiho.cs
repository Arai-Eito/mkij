using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Taiho : MonoBehaviour
{
    public GameObject _bulletPrefab;
    [SerializeField] int _number = 0;
    [SerializeField] int _bulletNumber = 50;
    [SerializeField] TMP_Text _text;

    bool _shotting = false;
    int _damage = 0;

    private void Start()
    {
        SetNumber(_bulletNumber);
        UpdateNumberText();
    }


    private void Update()
    
    {
        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            _shotting = true;
            StartCoroutine(_Shot());
        }
    }

    IEnumerator _Shot()
    {

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        int mask = LayerMask.GetMask("CameraRay");
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            Vector3 direction = hit.point - transform.position;
            direction.y = 0.0f;

            direction = direction.normalized;
            for (int i = 0; i < _bulletNumber; i++)
            {
                GameObject obj = Instantiate(_bulletPrefab);
                obj.transform.position = transform.position;

                Bullet b = obj.GetComponent<Bullet>();
                b.SetParameter(8, direction,_damage);


                yield return new WaitForSeconds(0.1f);
            }
        }

        _shotting = false;
        yield break;
    }


    public void SetNumber(int number)
    {
        _number = number;

        if(50 <= _number)
        {
            _bulletNumber = 50;
            _damage = _number / _bulletNumber;
        }
        else
        {
            _bulletNumber = _number;
            _damage = 1;
        }
    }

    public int GetNumber()
    {
        return _number;
    }
    public void UpdateNumberText()
    {
        _text.text = _number.ToString();        
    }
}
