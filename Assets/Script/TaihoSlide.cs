using UnityEngine;
using UnityEngine.InputSystem;

public class TaihoSlide : MonoBehaviour
{
    [Header("レイヤー")]
    [SerializeField] LayerMask cannonLayer; 
    [SerializeField] LayerMask railLayer;   

    [Header("大砲Transform")]
    [SerializeField] Transform railSpace;    // 土台(Base)

    [Header("移動制限")]
    [SerializeField] Transform leftLimit;    // Baseの子
    [SerializeField] Transform rightLimit;   // Baseの子

    [Header("追従速度")]
    [SerializeField] float smoothSpeed = 0f;

    Camera cam;
    bool dragging;
    float grabOffsetX;
    float minX, maxX;
    Vector3 fixedLocalYZ;

    void Awake()
    {
        cam = Camera.main;
        if (railSpace == null) railSpace = transform.parent;

        // YZ固定（ローカル）
        fixedLocalYZ = transform.localPosition;

        // 端点からmin/max
        float lx = railSpace.InverseTransformPoint(leftLimit.position).x;
        float rx = railSpace.InverseTransformPoint(rightLimit.position).x;
        minX = Mathf.Min(lx, rx);
        maxX = Mathf.Max(lx, rx);
    }

    void Update()
    {
        if (Mouse.current == null || cam == null) return;

        bool down = Mouse.current.leftButton.wasPressedThisFrame;
        bool up = Mouse.current.leftButton.wasReleasedThisFrame;
        bool held = Mouse.current.leftButton.isPressed;

        if (down && ClickedCannon())
        {
            dragging = true;

            // 掴んだ瞬間：レール上のヒット点から offset を取る
            if (TryGetRailLocalX(out float railX))
                grabOffsetX = transform.localPosition.x - railX;
            else
                grabOffsetX = 0f; // レールに当たらない時の保険
        }

        if (up) dragging = false;
        if (!dragging || !held) return;

        if (!TryGetRailLocalX(out float railLocalX)) return;

        float targetX = Mathf.Clamp(railLocalX + grabOffsetX, minX, maxX);

        Vector3 targetLocal = new Vector3(targetX, fixedLocalYZ.y, fixedLocalYZ.z);

        if (smoothSpeed <= 0f)
            transform.localPosition = targetLocal;
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocal, Time.deltaTime * smoothSpeed);
    }

    bool ClickedCannon()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Cannonレイヤーだけで判定（レールが手前でも無視できる）
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, cannonLayer))
            return hit.transform == transform || hit.transform.IsChildOf(transform);

        return false;
    }

    bool TryGetRailLocalX(out float railLocalX)
    {
        railLocalX = 0f;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Railレイヤーだけに当てる（レールのコライダーに当たった点を使う）
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, railLayer))
        {
            railLocalX = railSpace.InverseTransformPoint(hit.point).x;
            return true;
        }
        return false;
    }
}
