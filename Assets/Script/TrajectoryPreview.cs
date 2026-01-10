using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    [SerializeField] float _maxDistance = 20f;
    [SerializeField] float _radius = 0.25f;
    [SerializeField] LayerMask _collisionLayer;
    [SerializeField] Transform _endLineEffect;
    [SerializeField] LineRenderer _line1;
    [SerializeField] LineRenderer _line2;
    private void Awake()
    {

    }

    public void Draw(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector3 startPos = transform.position;

        direction.y = 0f;
        direction.Normalize();

        _line1.SetPosition(0, startPos);

        if (Physics.SphereCast(
            startPos,
            _radius,
            direction,
            out RaycastHit hit1,
            _maxDistance,
            _collisionLayer))
        {
            Vector3 p1 = hit1.point - direction * _radius;
            if (p1.z < 0) p1.z = 0;
            _line1.SetPosition(1, p1 + direction * 0.05f);
            _line2.SetPosition(0, p1);
            Vector3 normal = hit1.normal;
            normal.y = 0f;
            normal.Normalize();

            Vector3 reflectDir = Vector3.Reflect(direction, hit1.normal);
            reflectDir.y = 0f;
            reflectDir.Normalize();

            Vector3 bounceStart = p1 + reflectDir * 0.01f;

            if (Physics.SphereCast(
                bounceStart,
                _radius,
                reflectDir,
                out RaycastHit hit2,
                _maxDistance,
                _collisionLayer))
            {
                Vector3 p2 = hit2.point;
                if (p2.z < 0) p2.z = 0;

                float backOffset = 0.15f;
                p2 -= reflectDir * backOffset;
                _line2.SetPosition(1, p2);

                if (_endLineEffect != null)
                    _endLineEffect.position = p2;
            }
            else
            {
                _line2.SetPosition(1, bounceStart + reflectDir * _maxDistance);
            }
        }
        else
        {
            Vector3 end = startPos + direction * _maxDistance;
            _line1.SetPosition(1, end);
            _line2.SetPosition(1, end);
        }
    }
    public void SetVisable(bool status)
    {
        _line1.gameObject.SetActive(status);
        _line2.gameObject.SetActive(status);
        _endLineEffect.gameObject.SetActive(status);
    }
}
