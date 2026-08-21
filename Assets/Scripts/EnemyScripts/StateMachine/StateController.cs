using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public IState CurrentState {  get; private set; }

    public Transform Player { get; private set; }
    
    public Vector2 EnemyToPlayer { get; private set; }
    public float DistanceToPlayer { get; private set; }

    public Animator Animator { get; private set; }

    public bool locked = false; // Locks the state

    [SerializeField] private TextMeshProUGUI debugText;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Player = FindFirstObjectByType<PlayerController>()?.transform;
        //Player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void ChangeState(IState newState)
    {
        CurrentState?.OnExit(this);
        CurrentState = newState;
        CurrentState.OnEntry(this);
        debugText.text = CurrentState.GetName();
    }

    private void Update()
    {
        if (locked)
        {
            return;
        }
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
            if (attack is Shoot)
            {
                CoroutineManager.Instance.Run(attack.Execute(transform.position, EnemyToPlayer));
            }
            else if (attack is Punch)
            {
                Debug.Log("bug punch");
                CoroutineManager.Instance.Run(attack.Execute(
                    origin: transform.position, 
                    target: new Vector3(50,150))); // x is range, y is width
            }
        }
    }

    private float RotateSpeed = 5f;

    public void RotateToPlayer()
    {
        Vector2 direction = Player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, RotateSpeed * Time.deltaTime);
    }


}
