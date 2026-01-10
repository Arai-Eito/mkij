using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    [SerializeField] LineRenderer _line;
    [SerializeField] float _maxDistance = 20f;
    [SerializeField] float _radius = 0.25f;
    [SerializeField] LayerMask _collisionLayer;
    [SerializeField] Transform _endLineEffect;

    private void Awake()
    {
        if (_line == null)
            _line = GetComponent<LineRenderer>();

        _line.positionCount = 3;
    }

    public void Draw(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            Clear();
            return;
        }

        Vector3 startPos = transform.position;

        direction.y = 0f;
        direction.Normalize();

        _line.SetPosition(0, startPos);

        if (Physics.SphereCast(
            startPos,
            _radius,
            direction,
            out RaycastHit hit1,
            _maxDistance,
            _collisionLayer))
        {
            Vector3 p1 = hit1.point- direction* _radius;
            if (p1.z < 0) p1.z = 0;
            _line.SetPosition(1, p1);

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

                _line.SetPosition(2, p2);

                if (_endLineEffect != null)
                    _endLineEffect.position = p2;
            }
            else
            {
                _line.SetPosition(2, bounceStart + reflectDir * _maxDistance);
            }
        }
        else
        {
            Vector3 end = startPos + direction * _maxDistance;
            _line.SetPosition(1, end);
            _line.SetPosition(2, end);
        }
    }

    public void Clear()
    {
        _line.positionCount = 0;
        if (_endLineEffect != null)
            _endLineEffect.gameObject.SetActive(false);
    }
}
