using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class Taiho : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] AudioClip _shootSound;
    [SerializeField] private Transform _startPos;
    public GameObject _bulletPrefab;
    [SerializeField] int _number = 0;
    [SerializeField] int _bulletNumber = 50;
    [SerializeField] TMP_Text _text;
    [SerializeField] GameObject _shootEffect;
    bool _shotting = false;
    int _damage = 0;
    List<GameObject> _bullets = new List<GameObject>();


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        SetNumber(_bulletNumber);
        UpdateNumberText();
    }

    private void FixedUpdate()
    {

        if(_bullets.Count <= 0)
        {
            _shotting = false;
        }
        else
        {
            if (_bullets[0] == null) _bullets.RemoveAt(0);
        }
    }


    public void Shot()
    {
        if (_shotting) return;

        _shotting = true;
        StartCoroutine(_Shot());
        
    }

    IEnumerator _Shot()
    {
        Instantiate(_shootEffect, _startPos.position, _startPos.rotation);
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
                Destroy(obj, 10.0f);
                _bullets.Add(obj);

                Bullet b = obj.GetComponent<Bullet>();
                b.SetParameter(8, direction,_damage);
                _audioSource.PlayOneShot(_shootSound);

                yield return new WaitForSeconds(0.1f);
            }
        }


        yield break;
    }


    public void SetNumber(int number)
    {
        _number = number;

        if(30 <= _number)
        {
            _bulletNumber = 30;
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

    public bool GetShotting() { return  _shotting; }
    public bool GetIsDead() { return _number <= 0; }
}
