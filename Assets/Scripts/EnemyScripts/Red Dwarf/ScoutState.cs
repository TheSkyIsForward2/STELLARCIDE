using UnityEngine;

public class ScoutState : IState
{
    private IState IdleState;
    private IState ShootState;

    private float MoveSpeed = 1.0f;

    public void SetStates(IState idle, IState shoot)
    {
        IdleState = idle;
        ShootState = shoot;
    }

    public void OnEntry(StateController controller)
    {
        // This will be called when first entering the state
        UnityEngine.Debug.Log("Entered scout state");
    }

    public void OnUpdate(StateController controller)
    {
        if (controller.DistanceToPlayer > 20)
        {
            controller.ChangeState(IdleState);
        }
        if (controller.DistanceToPlayer < 12)
        {
            controller.ChangeState(ShootState);
        }

        controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.Player.position, MoveSpeed * Time.deltaTime);


    }

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }
}
