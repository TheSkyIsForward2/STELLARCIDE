using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Head : MonoBehaviour
{
    private StateController Controller;

    private IdleState Idle;
    private ScoutState Scout;
    private LungeAttackState Lunge;
    private RetreatState Retreat;

    public Transform bodyBehind;

    private float maxAngle = 60f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controller = GetComponent<StateController>();

        Idle = new IdleState();
        Scout = new ScoutState();
        Lunge = new LungeAttackState();
        Retreat = new RetreatState();

        Idle.SetStates(Scout);
        Scout.SetStates(Idle, Lunge);
        Lunge.SetStates(Retreat);
        Retreat.SetStates(Scout);

        Scout.SetDistance(20, 3);

        Controller.ChangeState(Idle);
    }
    private void FixedUpdate()
    {
        if (!bodyBehind)
            return;

        Vector2 bodyToHead =
            (Vector2)transform.position - (Vector2)bodyBehind.position;

        if (bodyToHead.sqrMagnitude < 0.0001f)
            return;

        float referenceAngle = Mathf.Atan2(bodyToHead.y, bodyToHead.x) * Mathf.Rad2Deg;

        float currentAngle = Controller.rb.rotation;

        float difference = Mathf.DeltaAngle(referenceAngle, currentAngle);

        if (Mathf.Abs(difference) > maxAngle)
        {
            float clampedAngle =
                referenceAngle + Mathf.Clamp(difference, -maxAngle, maxAngle);

            Controller.rb.MoveRotation(clampedAngle);
        }
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
