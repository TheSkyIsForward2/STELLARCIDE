using System.Diagnostics;
using UnityEngine;

public class ChaseState : IState
{
    private IState ScoutState;

    float chaseSpeed = 5;
    float loseDistance = 8;

    public Attack punch;
    private GameObject self;

    public void SetStates(IState scout)
    {
        ScoutState = scout;
    }

    public void OnEntry(StateController controller)
    {
        // This will be called when first entering the state
        // UnityEngine.Debug.Log("Entering chase state");

        self = controller.gameObject;
        punch = new Punch(self,
            damage: new Damage(10, Damage.Type.PHYSICAL), 
            cooldown: 2f
        );
    }

    public void OnUpdate(StateController controller)
    {
        // Scouting out enemy
        if (PlayerLost(controller.transform.position, controller.Player.position))
        {
            controller.ChangeState(ScoutState);
        } else
        {
            controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.Player.position, chaseSpeed * Time.deltaTime);
        }

        if (controller.DistanceToPlayer < 4.5 && punch != null)
        {
            controller.AttackPlayer(punch);
        }
    }

    public void OnExit(StateController controller)
    {
        // This will be called on leaving the state
    }

    private bool PlayerLost(Vector2 pos, Vector2 target)
    {

        if (Vector2.Distance(pos, target) > loseDistance)
        {
            return true;
        }

        return false;
    }
}
