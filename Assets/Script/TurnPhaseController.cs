using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using unityroom.Api;
public enum TURN_PHASE
{
    NONE = 0,
    TAIHO_PUZZLE,
    TAIHO_WAIT,
    STAGE_UPDATE,
    GAMEOVER
};


public class TurnPhaseController : MonoBehaviour
{
    private TURN_PHASE _phase;

    [SerializeField] Taiho _taiho;
    [SerializeField] Animator _animator;
    [SerializeField] ScoreManager _scoreManager;

    private void Start()
    {
        _phase = TURN_PHASE.TAIHO_PUZZLE;
    }
    private void Update()
    {
        switch (_phase)
        {
            case TURN_PHASE.NONE:
                break;
            
            case TURN_PHASE.TAIHO_PUZZLE:
                if (Mouse.current.rightButton.wasReleasedThisFrame)
                {
                    _taiho.Shot();
                    _phase = TURN_PHASE.TAIHO_WAIT;
                }
                break;

            case TURN_PHASE.TAIHO_WAIT:
                if(_taiho.GetShotting() == false)
                {
                    _phase = TURN_PHASE.STAGE_UPDATE;
                }
                break;

            case TURN_PHASE.STAGE_UPDATE:
                Stage.instance.NextTurn();
                _phase = TURN_PHASE.TAIHO_PUZZLE;

                if (_taiho.GetIsDead() == true)
                {
                    _phase = TURN_PHASE.GAMEOVER;
                    _animator.SetTrigger("GameOver");
                }
                break;

            case TURN_PHASE.GAMEOVER:

                _scoreManager.UploadScore();

                if (Mouse.current.rightButton.wasReleasedThisFrame)
                {
                    SceneManager.LoadScene("SampleScene");
                }

                break;
        }

    }

    ////////////////////////////////////////////////////////
    /// UI BUTTON “™
    public void ItemReroll()
    {
        Debug.Log(_phase);

        if (_phase != TURN_PHASE.TAIHO_PUZZLE) return;

        ItemManager.instance.ItemReroll();
    }
    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Skip()
    {
        if(_phase ==  TURN_PHASE.TAIHO_PUZZLE || _phase == TURN_PHASE.TAIHO_WAIT)
        {
            _taiho.Skip();
            _phase = TURN_PHASE.STAGE_UPDATE;
        }
    }
}
