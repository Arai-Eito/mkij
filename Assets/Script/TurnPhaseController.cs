using UnityEngine;
using UnityEngine.InputSystem;

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

                if (_taiho.GetIsDead() == true) _phase = TURN_PHASE.GAMEOVER;
                break;

            case TURN_PHASE.GAMEOVER:
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
}
