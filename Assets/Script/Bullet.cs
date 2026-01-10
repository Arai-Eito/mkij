using Unity.VisualScripting.Antlr3.Runtime.Tree;
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



        float minAxiszHitDistance = 999.0f, minAxisxHitDistance = 999.0f;
        Block axiszHitBlock = null, axisxHitBlock = null;
        RaycastHit[] hits = Physics.SphereCastAll(
            transform.position,
            0.3f,
            vel,
            _size);
        foreach(RaycastHit h in hits)
        {
            if (h.collider == null) continue;

            Transform hitTrans = h.collider.transform;

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


            // 盾軸の方が近い
            if (Mathf.Abs(outHorizontal) < Mathf.Abs(outVertical))
            {
                if(h.distance <= minAxiszHitDistance)
                {
                    minAxiszHitDistance = h.distance;

                    // ブロックを取得
                    Block b = h.collider.gameObject.GetComponent<Block>();
                    axiszHitBlock = (b != null && b.GetBlockType() != BLOCK_TYPE.NOMOVE) ? b : null;
                }
            }
            // 横軸の方が近い
            else
            {
                if (h.distance <= minAxisxHitDistance)
                {
                    minAxisxHitDistance = h.distance;

                    // ブロックを取得
                    Block b = h.collider.gameObject.GetComponent<Block>();
                    axisxHitBlock = (b != null && b.GetBlockType() != BLOCK_TYPE.NOMOVE) ? b : null;
                }
            }


        }


        bool spawneffect = false;
        if (minAxisxHitDistance != 999.0f)
        {
            spawneffect = true;
            _velocity.x = -_velocity.x;
        }
        if (minAxiszHitDistance != 999.0f)
        {
            spawneffect = true;
            _velocity.z = -_velocity.z;
        }
        if (spawneffect)
        {
            SpawnEffect();
            _audioSource.Play();
        }


        if (axisxHitBlock != null && axisxHitBlock.GetBlockType() != BLOCK_TYPE.NOMOVE)
        {
            // ダメージ
            int damage = _damage;
            int health = axisxHitBlock.GetNumber() - damage;


            if (health <= 0)
            {
                damage = health;
                Stage.instance.DeleteBlock(axisxHitBlock.GetIndex());
            }
            else
            {
                axisxHitBlock.SetNumber(health);
                axisxHitBlock.UpdateNumberText();
                axisxHitBlock.Damaged(); // ダメージエフェクト
            }


            // スコア追加
            ScoreManager.instance.AddScore(damage);
        }
        if (axiszHitBlock != null && axiszHitBlock.GetBlockType() != BLOCK_TYPE.NOMOVE)
        {
            // ダメージ
            int damage = _damage;
            int health = axiszHitBlock.GetNumber() - damage;


            if (health <= 0)
            {
                damage = health;
                Stage.instance.DeleteBlock(axiszHitBlock.GetIndex());
            }
            else
            {
                axiszHitBlock.SetNumber(health);
                axiszHitBlock.UpdateNumberText();
                axiszHitBlock.Damaged(); // ダメージエフェクト
            }


            // スコア追加
            ScoreManager.instance.AddScore(damage);
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
