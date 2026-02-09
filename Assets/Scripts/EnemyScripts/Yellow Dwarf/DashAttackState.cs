using UnityEngine;

public class DashAttackState : IState
{
    public Attack dash;
    private GameObject self;

    private float DashDistance = 5.0f;

    private IState DashChaseState;
    public void SetStates(IState dashChase)
    {
        DashChaseState = dashChase;
    }
    public void OnEntry(StateController controller)
    {
        // This will be called when first entering the state
        UnityEngine.Debug.Log("Entering dash attack state");

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
        if (controller.DistanceToPlayer > 10)
        {
            controller.ChangeState(DashChaseState);
        }

        if (dash.IsReady())
        {
            Vector3 dashDirection = controller.EnemyToPlayer.normalized * DashDistance;
            CoroutineManager.Instance.Run(dash.Execute(controller.transform.position, controller.transform.position + dashDirection));
        }
    }

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }
}
