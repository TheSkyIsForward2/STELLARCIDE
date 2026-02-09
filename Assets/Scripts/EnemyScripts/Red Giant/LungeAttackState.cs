using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LungeAttackState : IState
{
    private IState ScoutState;

    private bool isAttacking = false;

    [Header("Attack Properties")]
    public float windupDistance = 1.2f;
    public float windupTime = 1.0f;
    public float lungeDistance = 3f;
    public float lungeTime = 0.15f;
    public float attackCooldown = 1.0f;

    Vector3 startPos;
    Vector3 backPos;
    Vector3 lungePos;

    public Attack punch;
    private GameObject self;
    public void SetStates(IState scout)
    {
        ScoutState = scout;
    }

    public void OnEntry(StateController controller)
    {
        Debug.Log("Entered lunge state");
        self = controller.gameObject;
        punch = new Punch(self,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 2f
        );

        startPos = controller.transform.position;
        backPos = startPos + controller.transform.up * windupDistance;
        lungePos = backPos - controller.transform.up * lungeDistance;
    }

    public void OnUpdate(StateController controller)
    {
        Debug.DrawRay(controller.transform.position, controller.transform.up, Color.green);
        Debug.DrawRay(controller.transform.position, -controller.transform.up, Color.red);

        if (!isAttacking)
        {
            controller.StartCoroutine(Attack(controller));
        }
    }

    public void OnExit(StateController controller) 
    {
        
    }

    private IEnumerator Attack(StateController controller)
    {
        Debug.Log("started attack");
        Debug.Log("lerping from " + startPos + " to " + backPos);
        isAttacking = true;

        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / windupTime;
            controller.transform.position = Vector3.Lerp(startPos, backPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / lungeTime;
            controller.transform.position = Vector3.Lerp(backPos, lungePos, t);
            yield return null;
        }
        if (punch != null)
        {
            controller.AttackPlayer(punch);
        }

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / lungeTime;
            controller.transform.position = Vector3.Lerp(lungePos, startPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        controller.ChangeState(ScoutState);
        Debug.Log("finished attacking");
    }
}
