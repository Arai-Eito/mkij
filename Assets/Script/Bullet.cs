using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] private float _destroyTime;

    [SerializeField] Vector3 _velocity;
    [SerializeField] float _size;

    [SerializeField] GameObject _hitEffectPrefab;
    [SerializeField] GameObject _destroyEffectPrefab;
    private AudioSource _audioSource;
    [SerializeField] AudioClip _hitEffect;
    int _damage;
    private static int _count = 0;
    private static int _maxCount = 100;
    private bool isInit = false;
    Rigidbody _rb;
    bool isDoubled;

    private void Awake()
    {
        _count++;
        if (_count >= _maxCount) Destroy(gameObject);
        isInit = true;  
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>(); 
        _rb.useGravity = false;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Destroy(gameObject, _destroyTime);
    }
    private void OnDestroy()
    {
        if(isInit) _count--;
        Instantiate(_destroyEffectPrefab, transform.position, transform.rotation);
    }
    private void FixedUpdate()
    {
        _rb.linearVelocity = _velocity.normalized * _speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        SpawnEffect();
        _audioSource.PlayOneShot(_hitEffect);
        ContactPoint contact = collision.contacts[0];

        _velocity = Vector3.Reflect(_velocity, contact.normal);
        _velocity.y = 0.0f;

        Block block = collision.collider.GetComponent<Block>();
        if (block != null)
        {
            int hp = block.GetNumber() - _damage;
            block.SetNumber(hp);
            block.UpdateNumberText();

            if (hp <= 0 && Stage.instance != null)
            {
                Stage.instance.DeleteBlock(block.GetIndex());
            }
;        }

<<<<<<< Updated upstream
=======

        if (transform.position.z < 0) Destroy(gameObject);
>>>>>>> Stashed changes
    }

    void SpawnEffect()
    {
        if (_hitEffectPrefab == null) return;

        GameObject fx = Instantiate(_hitEffectPrefab, transform.position, transform.rotation);
    }

    public void SetParameter(float speed , Vector3 velocity,int daamge)
    {
        _speed = speed;
        _velocity = velocity;
        _damage = daamge;
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }
    private void OnDrawGizmos()
    {
        Vector3 vel = _velocity;
        vel.y = 0.0f;
        vel = vel.normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, vel * _size);
    }
    public Vector3 GetVelocity()
    {
        return _rb.linearVelocity;
    }
    public float GetSpeed()
    {
        return _speed;
    }

    public int GetDamage()
    {
        return _damage;
    }

    public bool GetIsDouble()
    {
        return isDoubled;
    }
    public void SetIsDouble(bool b)
    {
        isDoubled = b;
    }
}
