using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RetreatState : IState
{
    private IState ScoutState;
    private float MoveSpeed = 5.0f;
    private float turnDirection = 90f;

    private Vector3 retreatPoint;

    public void SetStates(IState scout)
    {
        ScoutState = scout;
    }

    public void OnEntry(StateController controller)
    {
        controller.StartCoroutine(ChangeState(controller));
        turnDirection *= Random.value < 0.5f ? -1 : 1;
    }

    public void OnUpdate(StateController controller)
    {

        controller.rb.linearVelocity = controller.transform.right * MoveSpeed;
        controller.RotateToPlayer();
        float nextAngle = Mathf.MoveTowardsAngle(controller.rb.rotation, controller.rb.rotation + 30f, 180f * Time.fixedDeltaTime);
        RotateAwayFromPlayer(controller);


        //controller.transform.position = Vector2.MoveTowards(controller.transform.position, controller.Player.position, MoveSpeed * Time.deltaTime);
    }

    private void RotateAwayFromPlayer(StateController controller)
    {
        Vector2 direction = controller.Player.position - controller.transform.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + turnDirection;

        float nextAngle = Mathf.MoveTowardsAngle(controller.rb.rotation, targetAngle, 180f * Time.fixedDeltaTime);

        // Apply the physical rotation smoothly
        controller.rb.MoveRotation(nextAngle);
    }

    public void OnExit(StateController controller)
    {

    }

    private IEnumerator ChangeState(StateController controller)
    {
        yield return new WaitForSeconds(5.0f);
        controller.ChangeState(ScoutState);
    }

    public string GetName()
    {
        return "Retreat Attack";
    }
}
