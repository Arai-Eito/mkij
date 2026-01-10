using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCursor : MonoBehaviour
{
    [SerializeField] private TurretRotation _turret;
    [SerializeField] Canvas _canvas;
    [SerializeField] RectTransform _cursor;
    [SerializeField] Stage _stage;

    private Vector3 _mouseLookPoint;
    public Transform _selectedParts;
    public Vector3 _selectedPartsStartPosition;
    public Vector3 _selectedPartsOffset;

    private void OnValidate()
    {
        _turret ??= FindAnyObjectByType<TurretRotation>();
    }
    private void FixedUpdate()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();

        {
            RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
            Vector2 canvasPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                null, // Overlay
                out canvasPos
            );

            // UI Image を移動
            _cursor.anchoredPosition = canvasPos;
        }

        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity,
                LayerMask.GetMask("CameraRay")))
            {

                _stage.GetIndex(hit.point);
                _mouseLookPoint = hit.point;
            }
        }

    }

    private void Update()
    {
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);


            int mask = LayerMask.GetMask("CameraRay");

            if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, mask))
            {
                _turret.LookAtPoint(hit2.point);
            }
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {

                if(_selectedParts == null)
                {
                    
                     mask = LayerMask.GetMask("Parts");

                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
                    {

                        _selectedParts = hit.collider.transform;                        
                        _selectedPartsStartPosition = _selectedParts.position;
                        _selectedPartsOffset = _selectedPartsStartPosition - hit.point;
                        
                    }
                    else
                    {
                        _selectedParts = null;
                    }
                }
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if(_selectedParts != null)
                {

                    Parts parts = _selectedParts.gameObject.GetComponent<Parts>();

                    if (parts.SetBlock(_stage))
                    {
                        // アイテムマネージャーのリストをクリアする
                        ItemManager.instance.ItemClear();

                        int childCount = _selectedParts.childCount;
                        for (int i = 0; i < childCount; i++)
                        {
                            _selectedParts.GetChild(0).SetParent(null);
                        }

                        Destroy(_selectedParts.gameObject);
                        _stage.CheckLineClear();
                    }
                    else
                    {
                        _selectedParts.position = _selectedPartsStartPosition;
                    }

                    _selectedParts = null;
                }
            }

            if(_selectedParts != null)
            {
                 mask = LayerMask.GetMask("CameraRay");

                if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity, mask))
                {
                    _selectedParts.position = hit.point + new Vector3(0.0f,3.3f,-0.6f) + _selectedPartsOffset;
                }
            }

        }
    }

    public Vector3 GetMousePoint()
    {
        return _mouseLookPoint;
    }
}
