using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class LungeAttackState : IState
{
    private IState RetreatState;

    private bool isAttacking = false;

    [Header("Attack Properties")]
    public float windupDistance = 1.2f;
    public float windupTime = 1.0f;
    public float lungeDistance = 3f;
    public float lungeTime = 0.15f;
    public float attackCooldown = 1.0f;

    public Attack punch;
    private GameObject self;
    public void SetStates(IState retreat)
    {
        RetreatState = retreat;
    }

    public void OnEntry(StateController controller)
    {
        self = controller.gameObject;
        controller.CurrentAttack = new Punch(self,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 2f
        );
    }

    public void OnUpdate(StateController controller)
    {
        Debug.DrawRay(controller.transform.position, -controller.transform.right, Color.green);
        Debug.DrawRay(controller.transform.position, controller.transform.right, Color.red);

        if (!isAttacking)
        {
            controller.StartCoroutine(Attack(controller));
        } else
        {
            controller.RotateToPlayer();
        }
    }

    public void OnExit(StateController controller) 
    {
        
    }

    private IEnumerator Attack(StateController controller)
    {
        // Debug.Log("started attack");
        // Debug.Log("lerping from " + startPos + " to " + backPos);
        isAttacking = true;
        yield return new WaitForSeconds(0.1f);
        controller.rb.linearVelocity = Vector2.zero;

        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime / windupTime;
            controller.rb.linearVelocity = -controller.transform.right * 3;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / lungeTime;
            controller.rb.linearVelocity = controller.transform.right * 50;
            yield return null;
        }

        if (controller.CurrentAttack != null)
        {
            if (controller.CurrentAttack.IsReady())
            {
                controller.TryTriggerAnimation("triggerAttack");
                controller.AttackPlayer();
            }
        }

        //yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;

        controller.ChangeState(RetreatState);
        // Debug.Log("finished attacking");
    }

    public string GetName()
    {
        return "Lunge Attack";
    }
}
