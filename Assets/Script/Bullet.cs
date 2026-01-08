using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _speed;

    [SerializeField] Vector3 _velocity;
    [SerializeField] float _size;
    [SerializeField] GameObject _hitEffectPrefab;

    private AudioSource _audioSource;
    [SerializeField] AudioClip _hitEffect;

    int _damage;

    private void Awake()
    {

        _audioSource = GetComponent<AudioSource>();

    }
    private void FixedUpdate()
    {
        transform.position += (Vector3)_velocity * _speed * Time.deltaTime;

        Vector3 vel = _velocity;
        vel.y = 0.0f;
        vel = vel.normalized;

        if (Physics.BoxCast(
            transform.position,
            new Vector3(0.2f, 0.2f, 0.2f),
            vel,
            out RaycastHit hit,
            Quaternion.identity,
            _size))
        {
            Transform hitTrans = hit.collider.transform;

            Vector3 direction = transform.position - hitTrans.position;
            direction.y = 0.0f;
            direction = direction.normalized;

            Vector3 axis;
            axis = hitTrans.right;
            axis.y = 0.0f;
            axis = axis.normalized;
            float outHorizontal = Vector3.Dot(direction, axis);
            axis = hitTrans.forward;
            axis.y = 0.0f;
            axis = axis.normalized;
            float outVertical = Vector3.Dot(direction, axis);


            SpawnEffect();
            _audioSource.PlayOneShot(_hitEffect);

            // 盾軸の方が近い
            if (Mathf.Abs(outHorizontal) < Mathf.Abs(outVertical))
            {
                _velocity.z = Mathf.Sign(direction.z) * Mathf.Abs(_velocity.z);
            }
            // 横軸の方が近い
            else
            {
                _velocity.x = Mathf.Sign(direction.x) * Mathf.Abs(_velocity.x);
            }

            // ダメージ
            // NOMOVEにはダメージを与えない
            Block b = hit.collider.gameObject.GetComponent<Block>();
            if (b != null && b.GetBlockType() != BLOCK_TYPE.NOMOVE)
            {
                int health = b.GetNumber() - _damage;
                b.SetNumber(health);
                b.UpdateNumberText();

                if (health <= 0)
                {
                    Stage.instance.DeleteBlock(b.GetIndex());
                }
            }
        }


        if (transform.position.z < 0) Destroy(gameObject);
    }
    void SpawnEffect()
    {
        if (_hitEffectPrefab == null) return;

        GameObject fx = Instantiate(_hitEffectPrefab, transform.position, transform.rotation);
    }
    public void SetParameter(float speed, Vector3 velocity, int daamge)
    {
        _speed = speed;
        _velocity = velocity;
        _damage = daamge;
    }

    private void OnDrawGizmos()
    {
        Vector3 vel = _velocity;
        vel.y = 0.0f;
        vel = vel.normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, vel * _size);
    }
}
