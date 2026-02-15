using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class IdleState : IState
{
    private IState ScoutState;

    public void SetStates(IState scout)
    {
        ScoutState = scout;
    }
    public void OnEntry(StateController controller)
    {
        // This will be called when first entering the state
        // UnityEngine.Debug.Log("Entering idle state");
    }

    public void OnUpdate(StateController controller)
    {
        if (controller.DistanceToPlayer < 20)
        {
            controller.ChangeState(ScoutState);
        }
    }

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }
}
