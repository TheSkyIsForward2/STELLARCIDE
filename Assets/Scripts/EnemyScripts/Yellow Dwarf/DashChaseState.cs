using System.Collections;
using UnityEngine;

public class DashChaseState : IState
{
    public Attack dash;
    private GameObject self;

    private float MoveSpeed = 5.0f;

    private float DashDistance = 3.0f;
    private bool attacking = false;

    private IState DashAttackState;
    public void SetStates(IState dashAttack)
    {
        DashAttackState = dashAttack;
    }
    public void OnEntry(StateController controller)
    {
        self = controller.gameObject;
        if (dash != null)
            return;

        dash = new Dash(self,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 3f,
            travelSpeed: 0.25f,
            lifetime: 1f,
            minDistance: 100f,
            maxDistance: 100f
            );
        controller.CurrentAttack = controller.dashAttack;

    }

    public void OnUpdate(StateController controller)
    {
        if (controller.DistanceToPlayer < 5)
        {
            controller.ChangeState(DashAttackState);
        }
        controller.rb.linearVelocity = Vector2.zero;

        if (controller.CurrentAttack != null && controller.CurrentAttack.IsReady())
        {
            Attack(controller);
        }
        else
        {
            controller.rb.linearVelocity = controller.transform.right * MoveSpeed;
            controller.RotateToPlayer();
        }
    }

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }

    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
            );
    }

    public string GetName()
    {
        return "Dash Chase";
    }

    private void Attack(StateController controller)
    {
        Vector3 dashDirection = rotate(controller.EnemyToPlayer,
            Random.Range(-45.0f, 45.0f) * Mathf.Deg2Rad).normalized * DashDistance;
        controller.AttackPlayer(controller.transform.position, controller.transform.position + dashDirection);
    }
}
