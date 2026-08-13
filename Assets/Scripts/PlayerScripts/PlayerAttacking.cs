using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Controller script for player attacks 
/// </summary>
public class PlayerAttacking : MonoBehaviour
{
    private PlayerController pc;
    #region Initialization
    [NonSerialized] public Attack PrimaryAttack;
    [NonSerialized] public Attack SecondaryAttack;

    private Punch punchAttack;
    private Shoot shootAttack;
    private Dash dashAttack;
    private Slash slashAttack;
    private Strafe strafeAttack;
    private Missile missileAttack;

    UpgradeData slashUpgradeData;
    UpgradeData missileUpgradeData;
    UpgradeData doublingUpgradeData;
    private PlayerControls inputActions;

    void Awake()
    {
        punchAttack = new Punch(gameObject,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 0.5f,
            travelSpeed: 10
        );
        shootAttack = new Shoot(gameObject,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 30,
            lifetime: 2,
            piercing: true
        );
        dashAttack = new Dash(gameObject, 
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed:0.25f, // looks like the max value before there is a pause after a dash
            lifetime: 1f
        );
        slashAttack = new Slash(gameObject,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed:10
        );
        strafeAttack = new Strafe(gameObject,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 3f,
            strafeStrength: 10f
        );
        missileAttack = new Missile(gameObject,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 10f,
            piercing: false,
            lifetime: 4f,
            homing: true
        );
        PrimaryAttack = shootAttack;
        SecondaryAttack = strafeAttack;

        slashUpgradeData = new UpgradeData(
            cooldown: 10f,
            duration: 5f
        );

        missileUpgradeData = new UpgradeData(
            cooldown: 10f,
            duration: 5f
        );

        doublingUpgradeData = new UpgradeData(
            cooldown: 10f,
            duration: 5f
        );

        inputActions = new PlayerControls();
        inputActions.Enable();

        // Primary Upgrades
        inputActions.Gameplay.UpgradeA.performed += (ctx) =>
        {
            switch (pc.GetPlayerMode())
            {
                case PlayerMode.MECH:
                    if (slashUpgradeData.IsReady())
                        StartCoroutine(ExecuteSlashUpgrade());
                    break;
                case PlayerMode.SHIP:
                    if (missileUpgradeData.IsReady())
                        StartCoroutine(ExecuteMissileUpgrade());
                    break;
            }
        };
        
        inputActions.Gameplay.UpgradeB.performed += (ctx) =>
        {
            if (doublingUpgradeData.IsReady())
            {
                StartCoroutine(ExecuteDoublingUpgrade());
            }
        };
    }


    void Start()
    {
        // this script now observes whenever the player changes forms and switches attacks accordingly
        EventBus.Instance.OnFormChange += (newMode) => SwapAttacks(newMode);
        pc = GetComponent<PlayerController>();
    }
    #endregion

    #region Input Polling
    void Update()
    {
        if (inputActions.Gameplay.PrimaryAttack.IsPressed())
        {
            // this is how you actually attack
            if (PrimaryAttack.IsReady()) // check if in cooldown
            {
                if (PrimaryAttack is Punch)
                {
                    StartCoroutine(PrimaryAttack.Execute(gameObject.transform.position, 
                        new Vector3(3,3)));
                }
                else
                {
                    StartCoroutine(PrimaryAttack.Execute(gameObject.transform.position, 
                        gameObject.transform.right));
                }
            }
        }

        if (inputActions.Gameplay.SecondaryAttack.IsPressed())
        {
            if (SecondaryAttack.IsReady())
            {
                if (SecondaryAttack is Dash)
                {
                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mouseWorldPos.z = transform.position.z;
                    StartCoroutine(SecondaryAttack.Execute(gameObject.transform.position, mouseWorldPos));
                }
                else if (SecondaryAttack is Strafe)
                {
                    Vector3 direction = Vector3.zero;
                    if (inputActions.Gameplay.Move.ReadValue<Vector2>().x < 0) // Strafing feels a little unintuitive right now when rotated
                    {
                        direction = gameObject.transform.up;
                    } else if (inputActions.Gameplay.Move.ReadValue<Vector2>().x > 0)
                    {
                        direction = -gameObject.transform.up;
                    }
                    if (direction == Vector3.zero) { return; }
                    StartCoroutine(SecondaryAttack.Execute(gameObject.transform.position, direction.normalized));
                    Debug.Log("strafe performed");
                }
            }
        }
    }
    #endregion


    void SwapAttacks(PlayerMode newMode)
    {
        switch (newMode)
        {
            case PlayerMode.SHIP:
                PrimaryAttack = shootAttack;
                SecondaryAttack = strafeAttack;
                break;
            default:
                PrimaryAttack = punchAttack;
                SecondaryAttack = dashAttack;
                break;
        }
    }


    IEnumerator ExecuteSlashUpgrade()
    {
        if (PrimaryAttack is Punch)
        {
            slashUpgradeData.IsActive = true;
            slashUpgradeData.LastExecute = Time.time;
            PrimaryAttack = slashAttack;
            if (doublingUpgradeData.IsActive) // check if doublingUpgrade is active
            {
                PrimaryAttack.Doubling = true;
            }
            yield return new WaitForSeconds(slashUpgradeData.Duration);
            PrimaryAttack = punchAttack;
            slashUpgradeData.IsActive = false;
        } 
    }
    
    IEnumerator ExecuteMissileUpgrade()
    {
        if (PrimaryAttack is Shoot)
        {
            missileUpgradeData.IsActive = true;
            missileUpgradeData.LastExecute = Time.time;
            PrimaryAttack = missileAttack;
            yield return new WaitForSeconds(missileUpgradeData.Duration);
            PrimaryAttack = shootAttack;
            missileUpgradeData.IsActive = false;
        } 
    }

    IEnumerator ExecuteDoublingUpgrade()
    {
        doublingUpgradeData.IsActive = true;
        doublingUpgradeData.LastExecute = Time.time;
        PrimaryAttack.Doubling = true;
        yield return new WaitForSeconds(doublingUpgradeData.Duration);
        slashAttack.Doubling = false; // not the best but o well
        punchAttack.Doubling = false;
        shootAttack.Doubling = false;
        doublingUpgradeData.IsActive = false;
    }
}