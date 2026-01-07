using UnityEngine;

public class TurretRotation : MonoBehaviour
{
    [SerializeField] float _rotateSpeed = 10f;
    public void LookAtPoint(Vector3 worldPoint)
    {
        Vector3 dir = worldPoint - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.001f)
            return;

        float targetY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(-90f, targetY, 0f);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * _rotateSpeed
        );
    }

}
