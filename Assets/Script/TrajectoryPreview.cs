using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private float _maxDistance = 20f;
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private GameObject _endLineEffect;
    [SerializeField] Vector3 _halfExtents = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] float _skin = 0.01f; 
    public void Draw( Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f) { Clear(); return; }

        Vector3 startPos = transform.position;

        direction.y = 0f;
        direction.Normalize();

        _line.SetPosition(0, startPos);

        // 1-й BoxCast
        if (Physics.BoxCast(startPos, _halfExtents, direction, out RaycastHit hit1,
                Quaternion.identity, _maxDistance, _collisionLayer))
        {
            Vector3 p1 = hit1.point;
            if (p1.z < 0) p1.z = 0;
            _line.SetPosition(1, p1);

            // ✅ Считаем отскок ТОЧНО как в Bullet (по X или Z)
            Vector3 bouncedDir = CalcGridBounceDir(direction, hit1);

            // старт 2-го каста чуть вперед по направлению отскока
            Vector3 bounceStart = hit1.point + bouncedDir * _skin;

            // 2-й BoxCast
            if (Physics.BoxCast(bounceStart, _halfExtents, bouncedDir, out RaycastHit hit2,
                    Quaternion.identity, _maxDistance, _collisionLayer))
            {
                Vector3 p2 = hit2.point;
                if (p2.z < 0) p2.z = 0;
                _line.SetPosition(2, p2);
            }
            else
            {
                _line.SetPosition(2, bounceStart + bouncedDir * _maxDistance);
            }
        }
        else
        {
            Vector3 end = startPos + direction * _maxDistance;
            _line.SetPosition(1, end);
            _line.SetPosition(2, end);
        }
    }
    Vector3 CalcGridBounceDir(Vector3 inDir, RaycastHit hit)
    {
        Transform hitTrans = hit.collider.transform;

        // direction = bulletPos - hitPos (как у тебя)
        Vector3 direction = hit.point - hitTrans.position;
        direction.y = 0f;
        direction.Normalize();

        Vector3 axisR = hitTrans.right; axisR.y = 0f; axisR.Normalize();
        float outHorizontal = Vector3.Dot(direction, axisR);

        Vector3 axisF = hitTrans.forward; axisF.y = 0f; axisF.Normalize();
        float outVertical = Vector3.Dot(direction, axisF);

        Vector3 outDir = inDir;

        if (Mathf.Abs(outHorizontal) < Mathf.Abs(outVertical))
        {
            outDir.z = Mathf.Sign(direction.z) * Mathf.Abs(outDir.z);
        }
        else
        {
            outDir.x = Mathf.Sign(direction.x) * Mathf.Abs(outDir.x);
        }

        outDir.y = 0f;
        return outDir.normalized;
    }
    public void Clear()
    {
        _line.positionCount = 0;
    }
}
