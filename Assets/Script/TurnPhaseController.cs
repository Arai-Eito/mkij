using UnityEngine;

public enum TURN_PHASE
{
    NONE = 0,
    TAIHO_PUZZLE,
    STAGE_UPDATE,
};


public class TurnPhaseController : MonoBehaviour
{
    private TURN_PHASE _phase;


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
                break;

            case TURN_PHASE.STAGE_UPDATE:
                break;
        }

    }
}
