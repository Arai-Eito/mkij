using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _speed;

    [SerializeField] Vector3 _velocity;
    [SerializeField] GameObject _hitEffectPrefab;

    private AudioSource _audioSource;
    [SerializeField] AudioClip _hitEffect;

    int _damage;

    private void Awake()
    {

        _audioSource = GetComponent<AudioSource>();

        _isMoving = false;
    }

    private void Update()
    {
        /*
        Vector3 velocity = (Vector3)_velocity * _speed * Time.deltaTime;
        if (transform.position.z < -0.3f) Destroy(gameObject);

        float _size = velocity.magnitude;

        if (3.0f < Mathf.Abs(transform.position.x))
        {
            Debug.Log("hanten ");
            _velocity.x = -_velocity.x;


        }


        // x
        if (Physics.Raycast(
            transform.position,
            Vector3.right * Mathf.Sign(velocity.x),
            out RaycastHit hitx,
            _size))
        {
            float distance = transform.position.x - hitx.collider.transform.position.x;

            if (Mathf.Sign(_velocity.x) != Mathf.Sign(distance))
            {
                // ブロックを取得
                Block b = hitx.collider.gameObject.GetComponent<Block>();
                if (b != null && b.GetBlockType() != BLOCK_TYPE.NOMOVE)
                {

                    // ダメージ
                    int damage = _damage;
                    int health = b.GetNumber() - damage;


                    if (health <= 0)
                    {
                        damage = health;
                        Stage.instance.DeleteBlock(b.GetIndex());
                    }
                    else
                    {
                        b.SetNumber(health);
                        b.UpdateNumberText();
                        b.Damaged(); // ダメージエフェクト
                    }


                    // スコア追加
                    ScoreManager.instance.AddScore(damage);
                }

                // 方向変えて
                _velocity.x = -_velocity.x;

                // エフェクト出す
                SpawnEffect();

            }
        }

        if (Physics.Raycast(
            transform.position,
            Vector3.forward * Mathf.Sign(velocity.z),
            out RaycastHit hitz,
            _size))
        {
            float distance = transform.position.z - hitz.collider.transform.position.z;

            if (Mathf.Sign(velocity.z) != Mathf.Sign(distance))
            {
                // ブロックを取得
                Block b = hitz.collider.gameObject.GetComponent<Block>();
                if (b != null && b.GetBlockType() != BLOCK_TYPE.NOMOVE)
                {

                    // ダメージ
                    int damage = _damage;
                    int health = b.GetNumber() - damage;


                    if (health <= 0)
                    {
                        damage = health;
                        Stage.instance.DeleteBlock(b.GetIndex());
                    }
                    else
                    {
                        b.SetNumber(health);
                        b.UpdateNumberText();
                        b.Damaged(); // ダメージエフェクト
                    }


                    // スコア追加
                    ScoreManager.instance.AddScore(damage);
                }

                // 方向変えて
                _velocity.z = -_velocity.z;

                // エフェクト出す
                SpawnEffect();

            }
        }


        transform.position += velocity;
        */

        if(_isMoving == true)
        {
            if (transform.position.z < -0.3f) Destroy(gameObject);
        }
        else
        {
            if(Physics.Raycast(
                transform.position,
                _velocity.normalized,
                out RaycastHit hit,
                Mathf.Infinity))
            {

                _nextVelocity = Vector3.Reflect(_velocity, hit.normal);
                _targetBlock = hit.collider.gameObject.GetComponent<Block>();
                _isTargetBlock = _targetBlock != null;
                _isMoving = true;


                StartCoroutine(Move(hit.point));
            }
                
        }
    }




    void SpawnEffect()
    {
        if (_hitEffectPrefab == null) return;

        GameObject fx = Instantiate(_hitEffectPrefab, transform.position, transform.rotation);
    }
    public void SetParameter(float speed, Vector3 velocity, int damage)
    {
        _speed = speed;
        _velocity = velocity;
        _damage = damage;
    }



    bool _isMoving = false;

    bool _isTargetBlock;
    Block _targetBlock;

    Vector3 _nextVelocity;

    IEnumerator Move(Vector3 targetPosition)
    {

        while(true)
        {
            Vector3 direction = targetPosition - transform.position;
            float length = direction.magnitude;
            float speed = _speed * Time.deltaTime;
            direction = direction.normalized;

            if (length < speed)
            {
                transform.position = targetPosition;
                break;
            }
            else
            {
                transform.position += direction * speed;
            }

            yield return null ;
        }

        // ダメーじ

        // ブロック
        if(_targetBlock != null && _targetBlock.GetBlockType() == BLOCK_TYPE.MOVE)
        {
            // ダメージ
            int damage = _damage;
            int health = _targetBlock.GetNumber() - damage;


            if (health <= 0)
            {
                damage = health;
                Stage.instance.DeleteBlock(_targetBlock.GetIndex());
            }
            else
            {
                _targetBlock.SetNumber(health);
                _targetBlock.UpdateNumberText();
                _targetBlock.Damaged(); // ダメージエフェクト
            }


            // スコア追加
            ScoreManager.instance.AddScore(damage);
        }

        // 方向変えて
        if (_targetBlock != null || _isTargetBlock == false)
        {
            _velocity = _nextVelocity;
            // エフェクト出す
            SpawnEffect();
        }


        // 終わり
        _isMoving = false;

        yield break;
    }
}
