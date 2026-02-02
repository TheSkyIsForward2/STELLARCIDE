using Unity.VisualScripting;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public IState CurrentState {  get; private set; }

    public Transform Player { get; private set; }
    
    public Vector2 EnemyToPlayer { get; private set; }
    public float DistanceToPlayer { get; private set; }

    private void Awake()
    {
        //Player = FindFirstObjectByType<PlayerController>()?.transform;
        Player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void ChangeState(IState newState)
    {
        CurrentState?.OnExit(this);
        CurrentState = newState;
        CurrentState.OnEntry(this);
    }

    private void Update()
    {
        if (Player == null || CurrentState == null) return;

        EnemyToPlayer = Player.position - transform.position;
        DistanceToPlayer = EnemyToPlayer.magnitude;

        CurrentState.OnUpdate(this);
        RotateToPlayer();
    }

    public void AttackPlayer(Attack attack)
    {
        if (attack.IsReady())
        {
            CoroutineManager.Instance.Run(attack.Execute(transform.position, EnemyToPlayer));
            Debug.Log("trying to dash");
        }
    }

    private float RotateSpeed = 5f;

    public void RotateToPlayer()
    {
        Vector2 direction = Player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, RotateSpeed * Time.deltaTime);
    }


}
