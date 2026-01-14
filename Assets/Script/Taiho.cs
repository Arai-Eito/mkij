using System.Collections;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Taiho : MonoBehaviour
{
    [SerializeField] private CameraCursor _cursor;

    public GameObject _bulletPrefab;
    [SerializeField] int _number = 0;
    [SerializeField] int _bulletNumber = 50;
    [SerializeField] float _angleLineZ = 0.75f;

    [SerializeField] TMP_Text _text;


    // mishanya
    // ëÂñCÇ©ÇÁèoÇÈÇ‚Ç¬
    [SerializeField] GameObject _shootEffect;
    [SerializeField] TurretRotation _turret;
    [SerializeField] AudioClip _shootSound;
    [SerializeField] private Transform _startPos;
    // ó\ë™ê¸
    private TrajectoryPreview _trajectory;
    private AudioSource _audioSource;



    bool _shooting = false;
    bool _skip = false;
    int _damage = 0;
    int _modDamage = 0;

    List<GameObject> _bullets = new List<GameObject>();
    private Vector3 _direction;


    private void OnValidate()
    {
        _cursor ??= GetComponent<CameraCursor>();
        _turret ??= FindAnyObjectByType<TurretRotation>();
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
        if(_shooting == false)
        {

            if (_bullets.Count > 0)
            {
                if (_bullets[0] == null) _bullets.RemoveAt(0);
            }
        }
    }

    private void Update()
    {
        if (_shooting)
        {
            _trajectory.SetVisable(false);
            return;
        }
        _trajectory.SetVisable(true);
        _direction = GetDiraction();
        _trajectory.Draw(_direction);

        _turret.LookAtPoint(_turret.transform.position + _direction);
    }

    /// <summary>
    /// íeÇë≈Ç¬ì¸ÇËå˚
    /// </summary>
    public void Shot()
    {
        if (_shooting) return;

        _shooting = true;
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
                _shooting = false;
                yield break;
            }

            CreateBullet(_damage);
            yield return delay;
        }

        if(_modDamage != 0)
        {
            CreateBullet(_modDamage);
        }

        _shooting = false;
        yield break;
    }
    private void CreateBullet(int damage)
    {
        GameObject obj = Instantiate(_bulletPrefab, _startPos.position, Quaternion.identity);

        Bullet b = obj.GetComponent<Bullet>();
        b.SetParameter(8f, _direction, damage);
        _audioSource.PlayOneShot(_shootSound);

        Destroy(obj, 10f);
        _bullets.Add(obj);
    }



    private Vector3 GetDiraction()
    {
        Vector3 cursorPos = _cursor.GetMousePoint();
        Vector3 dir = cursorPos - _startPos.position;
        dir.y = 0f;
        dir.Normalize();

        bool zline = (cursorPos.z < _angleLineZ);
        bool ougi = (Vector3.Dot(Vector3.forward, dir) > 0.3f);
        bool yoko = (Mathf.Abs(cursorPos.x) < 2.5f);

        if(ougi && zline == false)
        {
        }
        else if(yoko)
        {
            cursorPos.z = _angleLineZ;
        }
        else
        {
            return _direction;
        }

        dir = cursorPos - _startPos.position;
        dir.y = 0f;
        dir.Normalize();

        return dir;
    }

    public void SetNumber(int number)
    {
        _number = number;

        if(30 <= _number)
        {
            _bulletNumber = 30;
            _damage = _number / _bulletNumber;
            _modDamage = _number % _bulletNumber;
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
    public bool GetWait() { return  _shooting || _bullets.Count > 0; }
    public bool GetIsDead() { return _number <= 0; }
}
