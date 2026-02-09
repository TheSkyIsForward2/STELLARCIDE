using UnityEngine;

public class DashChaseState : IState
{
    public Attack dash;
    private GameObject self;

    private float DashDistance = 3.0f;

    private IState DashAttackState;
    public void SetStates(IState dashAttack)
    {
        DashAttackState = dashAttack;
    }
    public void OnEntry(StateController controller)
    {
        // This will be called when first entering the state
        UnityEngine.Debug.Log("Entering dash chase state");

        self = controller.gameObject;
        if (dash != null)
            return;

        dash = new Dash(self,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 0.25f,
            lifetime: 1f
            );

    }

    public void OnUpdate(StateController controller)
    {
        if (controller.DistanceToPlayer < 10)
        {
            controller.ChangeState(DashAttackState);
        }

        if (dash.IsReady())
        {
            Vector3 dashDirection = rotate(controller.EnemyToPlayer, 
                Random.Range(-45.0f, 45.0f) * Mathf.Deg2Rad).normalized * DashDistance;
            CoroutineManager.Instance.Run(dash.Execute(controller.transform.position, controller.transform.position + dashDirection));
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
}
