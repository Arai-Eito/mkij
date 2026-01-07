using UnityEngine;

public class UpgardeWall : MonoBehaviour
{
    [SerializeField] float _speed = 5f;

    [SerializeField] private Transform _target1, _target2;
    private Vector3 _currentTurget;

    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _splitAngle = 15f;
    private void Start()
    {
        _currentTurget = _target1.position;
    }
    private void Update()
    {
        Move();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Bullet bullet))
            return;
//        if(bullet.GetIsDouble())return;
//        Vector3 baseDir = bullet.GetVelocity().normalized;
//        float speed = bullet.GetSpeed();
//        int damage = bullet.GetDamage();

        Quaternion leftRot = Quaternion.AngleAxis(-_splitAngle, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(_splitAngle, Vector3.up);

        
 //       SpawnBullet(bullet.transform.position, leftRot * baseDir, speed, damage);
 //       SpawnBullet(bullet.transform.position, rightRot * baseDir, speed, damage);

        Destroy(bullet.gameObject);
    }

    private void SpawnBullet(Vector3 pos, Vector3 dir, float speed, int damage)
    {
        GameObject obj = Instantiate(_bulletPrefab, pos, Quaternion.identity);
        Bullet b = obj.GetComponent<Bullet>();
        b.SetParameter(speed, dir, damage);
//        b.SetIsDouble(true);
    }
    private void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _currentTurget,
            _speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _currentTurget) < 0.01f)
        {
            if (_currentTurget == _target1.position)
            _currentTurget = _target2.position;
            else _currentTurget = _target1.position;
        }
    }
}
