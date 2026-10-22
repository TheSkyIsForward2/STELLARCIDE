using System.Collections;
using UnityEngine;
public class DashAttackState : IState
{
    public Attack dash;
    private GameObject self;

    private float DashDistance = 5.0f;
    private bool attacking = false;
    private bool canAttack = true;

    private IState DashChaseState;
    public void SetStates(IState dashChase)
    {
        DashChaseState = dashChase;
    }

    public void OnEntry(StateController controller)
    {
        controller.CurrentAttack = controller.dashAttack;
    }

    public void OnUpdate(StateController controller)
    {
        controller.rb.linearVelocity = Vector2.zero;
        if (controller.DistanceToPlayer > 10)
        {
            controller.ChangeState(DashChaseState);
        }
        if (controller.CurrentAttack.IsReady())
        {
            Attack(controller);
        }
        controller.RotateToPlayer();
    }

    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
    }

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }

    public string GetName()
    {
        return "Dash Attack";
    }

    private void Attack(StateController controller)
    {
        //Vector3 dashDirection = (controller.Player.position - controller.transform.position) * 5;
        controller.AttackPlayer(controller.transform.position, controller.transform.position + controller.transform.right * 10);
    }

}
