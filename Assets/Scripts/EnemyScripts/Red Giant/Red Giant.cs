using System.Collections.Generic;
using UnityEngine;

public class RedGiant : MonoBehaviour
{
    private StateController Controller;

    private IdleState Idle;
    private ScoutState Scout;
    private LungeAttackState Lunge;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controller = GetComponent<StateController>();

        Idle = new IdleState();
        Scout = new ScoutState();
        Lunge = new LungeAttackState();

        Idle.SetStates(Scout);
        Scout.SetStates(Idle, Lunge);
        Lunge.SetStates(Scout);

        Scout.SetDistance(20, 3);

        Controller.ChangeState(Idle);
    }
}
