using System.Collections;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Taiho : MonoBehaviour
{
    [SerializeField] private CameraCursor _cursor;
    private TrajectoryPreview _trajectory;
    private AudioSource _audioSource;

    [SerializeField] AudioClip _shootSound;
    [SerializeField] private Transform _startPos;
    public GameObject _bulletPrefab;
    [SerializeField] int _number = 0;
    [SerializeField] int _bulletNumber = 50;
    [SerializeField] TMP_Text _text;
    [SerializeField] GameObject _shootEffect;
    bool _shotting = false;
    bool _skip = false;
    int _damage = 0;
    List<GameObject> _bullets = new List<GameObject>();
    private Vector3 _diraction;
    private void OnValidate()
    {
        _cursor ??= GetComponent<CameraCursor>();
    }
    private void Start()
    {
        _trajectory = GetComponent<TrajectoryPreview>();
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

    private void Update()
    {
        if (_shotting) return;

        _diraction = GetDiraction();
        _trajectory.Draw(_diraction);
    }

    public void Shot()
    {
        if (_shotting) return;

        _shotting = true;
        _skip = false;
        StartCoroutine(_Shot());
        
    }

    IEnumerator _Shot()
    {
        Instantiate(_shootEffect, _startPos.position, _startPos.rotation);
        _audioSource.PlayOneShot(_shootSound);

            WaitForSeconds delay = new WaitForSeconds(0.1f);

        for (int i = 0; i < _bulletNumber; i++)
        {
            if(_skip)
            {
                yield break;
            }
            GameObject obj = Instantiate(_bulletPrefab, _startPos.position, Quaternion.identity);

            Bullet b = obj.GetComponent<Bullet>();
            b.SetParameter(8f, _diraction, _damage);
            _audioSource.PlayOneShot(_shootSound);

            Destroy(obj, 10f);
            _bullets.Add(obj);
            yield return delay;
        }
        yield break;
    }

    private Vector3 GetDiraction()
    {
        Vector3 dir = _cursor.GetMousePoint() - _startPos.position;
        dir.y = 0f;

        Debug.Log(_cursor.GetMousePoint());

        if (dir.sqrMagnitude > 0.0001f)
        {
            dir.Normalize();
            return dir;
        }
        return Vector3.zero;
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

    public void Skip()
    {
        _skip = true;

        while(_bullets.Count != 0)
        {
            Destroy(_bullets[0]);
            _bullets.RemoveAt(0);
        }

    }
    public bool GetShotting() { return  _shotting; }
    public bool GetIsDead() { return _number <= 0; }
}
