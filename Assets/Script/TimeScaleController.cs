using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    int _cursor = 0;
    [SerializeField] GameObject[] _button;



    private void Start()
    {
        for(int i = 0; i < _button.Length; i++)
        {
            _button[i].SetActive(false);
        }
        _button[_cursor].SetActive(true);
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;

        _button[_cursor].SetActive(false);
        _cursor++;
        _cursor = _cursor % _button.Length;
        _button[_cursor].SetActive(true);
    }
}
