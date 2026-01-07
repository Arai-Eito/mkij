using UnityEngine;
using UnityEngine.InputSystem;

public class CameraCursor : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] RectTransform _cursor;
    [SerializeField] Stage _stage;

    private Transform _selectedParts;
    private Vector3 _selectedPartsStartPosition;
    private Vector3 _selectedPartsOffset;

    private void FixedUpdate()
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();

        // UI CURSOR
        {
            // Canvasローカル座標に変換
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
                1 << LayerMask.GetMask("CameraRay")))
            {
                _stage.GetIndex(hit.point);
            }
        }

    }

    private void Update()
    {
        // DRAG DROP
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            
            // CATCH
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {

                if(_selectedParts == null)
                {
                    
                    int mask = LayerMask.GetMask("Parts");

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

            // DROP
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if(_selectedParts != null)
                {

                    Parts parts = _selectedParts.gameObject.GetComponent<Parts>();

                    // 設置成功したら親だけ消す
                    if (parts.SetBlock(_stage))
                    {
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

            // DRAG
            if(_selectedParts != null)
            {
                int mask = LayerMask.GetMask("CameraRay");

                if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity, mask))
                {
                    _selectedParts.position = hit.point + _selectedPartsOffset;
                }
            }

        }
    }
}
