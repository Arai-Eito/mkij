using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private float _maxDistance = 20f;
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private GameObject _endLineEffect;

    public void Draw( Vector3 direction)
    {
        Debug.Log(1);
        Vector3 startPos = transform.position;
        direction.y = 0f;
        direction.Normalize();

        _line.positionCount = 3;

        if (Physics.Raycast(startPos, direction, out RaycastHit hit1, _maxDistance, _collisionLayer))
        {
            _line.SetPosition(0, startPos);
            var offset = hit1.point;
            if (offset.z < 0) offset.z = 0;
            _line.SetPosition(1, offset);

            Vector3 reflectDir = Vector3.Reflect(direction, hit1.normal);
            reflectDir.y = 0f;
            reflectDir.Normalize();

            if (Physics.Raycast(hit1.point, reflectDir, out RaycastHit hit2, _maxDistance, _collisionLayer))
            {
                var offset2 = hit2.point;
                if (offset2.z < 0) offset2.z = 0;
                _line.SetPosition(2, offset2);

                float backOffset = 0.2f;
                offset2 -= reflectDir * backOffset;
                offset2.y += backOffset;
                _line.SetPosition(2, offset2);
                _endLineEffect.transform.position = offset2;
            }
            else
            {
                _line.SetPosition(2, hit1.point + reflectDir * _maxDistance);
            }
        }
        else
        {
            _line.SetPosition(0, startPos);
            _line.SetPosition(1, startPos + direction * _maxDistance);
            _line.SetPosition(2, startPos + direction * _maxDistance);
        }
    }

    public void Clear()
    {
        _line.positionCount = 0;
    }
}
