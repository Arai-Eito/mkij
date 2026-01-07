using UnityEngine;

public class UpgardeWall : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _splitAngle = 15f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Bullet bullet))
            return;
        if(bullet.GetIsDouble())return;
        Vector3 baseDir = bullet.GetVelocity().normalized;
        float speed = bullet.GetSpeed();
        int damage = bullet.GetDamage();

        Quaternion leftRot = Quaternion.AngleAxis(-_splitAngle, Vector3.up);
        Quaternion rightRot = Quaternion.AngleAxis(_splitAngle, Vector3.up);

        
        SpawnBullet(bullet.transform.position, leftRot * baseDir, speed, damage);
        SpawnBullet(bullet.transform.position, rightRot * baseDir, speed, damage);

        Destroy(bullet.gameObject);
    }

    private void SpawnBullet(Vector3 pos, Vector3 dir, float speed, int damage)
    {
        GameObject obj = Instantiate(_bulletPrefab, pos, Quaternion.identity);
        Bullet b = obj.GetComponent<Bullet>();
        b.SetParameter(speed, dir, damage);
        b.SetIsDouble(true);
    }
}
