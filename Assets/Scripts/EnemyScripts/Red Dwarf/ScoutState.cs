using UnityEngine;

public class ScoutState : IState
{
    private IState IdleState;
    private IState ShootState;

    private float MoveSpeed = 5.0f;

    private float IdleSwitch = 20.0f;
    private float ShootSwitch = 12.0f;

    public void SetStates(IState idle, IState shoot)
    {
        IdleState = idle;
        ShootState = shoot;
    }

    public void SetDistance(float dist1, float dist2)
    {
        IdleSwitch = dist1;
        ShootSwitch = dist2;
    }

    public void OnEntry(StateController controller)
    {
        controller.TryTriggerAnimation("triggerWalk");
    }

    public void OnUpdate(StateController controller)
    {
        if (controller.DistanceToPlayer > IdleSwitch)
        {
            controller.ChangeState(IdleState);
        }
        if (controller.DistanceToPlayer < ShootSwitch)
        {
            controller.ChangeState(ShootState);
        }

        controller.rb.linearVelocity = controller.transform.right * MoveSpeed;
        controller.RotateToPlayer();
    }

    //private bool isFacingPlayer(StateController controller)
    //{
    //    Vector3 directionToTarget = (controller.Player.position - controller.transform.position).normalized;

    //    Vector3 forwardDirection = controller.transform.right;

    //    float dotProduct = Vector3.Dot(forwardDirection, directionToTarget);

    //    float precision = Mathf.Cos(2f * Mathf.Deg2Rad);
    //    return dotProduct > precision;
    //}

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }

    public string GetName()
    {
        return "Scout";
    }
}
