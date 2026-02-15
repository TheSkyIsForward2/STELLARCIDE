using System.Diagnostics;
using UnityEngine;

public class ShootState : IState
{
    public Attack shoot;
    private GameObject self;

    private IState ScoutState;
    private IState ChaseState;

    public void SetStates(IState scout, IState chase)
    {
        ScoutState = scout;
        ChaseState = chase;
    }

    public void OnEntry(StateController controller)
    {
        // This will be called when first entering the state
        // UnityEngine.Debug.Log("entering shooting state");

        self = controller.gameObject;
        if (shoot != null)
            return;
        
        shoot = new Shoot(self,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 10,
            lifetime: 2,
            piercing: false
        );
    }

    public void OnUpdate(StateController controller)
    {
        if (controller.DistanceToPlayer > 12)
        {
            controller.ChangeState(ScoutState);
        } else if (controller.DistanceToPlayer < 8)
        {
            controller.ChangeState(ChaseState);
        }
        // Scouting out enemy
        controller.AttackPlayer(shoot);
    }

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }
}
