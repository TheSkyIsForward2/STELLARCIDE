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

    [SerializeField] public GameObject chargeUpIndicator;

    public Punch punchAttack;
    public Shoot shootAttack;
    public Dash dashAttack;
    public Slash slashAttack;
    public Strafe strafeAttack;
    public Missile missileAttack;

    public bool ChargeUpActive = false;

    private PlayerControls inputActions;

    void Awake()
    {
        inputActions = new PlayerControls();
        inputActions.Enable();

        // Debug Heal
        inputActions.Gameplay.Heal.performed += (ctx) =>
        {
            GetComponent<PlayerHealth>().healthController.TakeDamage(new Damage(-100, Damage.Type.PHYSICAL));
        };

        // Primary Upgrades
        inputActions.Gameplay.UpgradeA.performed += (ctx) =>
        {
            if (UpgradeManager.Instance.PrimaryUpgrade == null) { return;  }
            if (UpgradeManager.Instance.PrimaryUpgrade.IsReady())
            {
                StartCoroutine(UpgradeManager.Instance.PrimaryUpgrade.Execute(this));
            }
        };
        
        inputActions.Gameplay.UpgradeB.performed += (ctx) =>
        {
            if (UpgradeManager.Instance.SecondaryUpgrade == null) { return; }
            if (UpgradeManager.Instance.SecondaryUpgrade.IsReady())
            {
                StartCoroutine(UpgradeManager.Instance.SecondaryUpgrade.Execute(this));
            }
        };
    }


    void Start()
    {
        // this script now observes whenever the player changes forms and switches attacks accordingly
        EventBus.Instance.OnFormChange += (newMode) => SwapAttacks(newMode);
        pc = GetComponent<PlayerController>();

        UpgradeManager instance = UpgradeManager.Instance;

        if (instance != null)
        {
            if (instance.hasPunch)
            {
                punchAttack = new Punch(gameObject,
                    damage: new Damage(10, Damage.Type.PHYSICAL),
                    cooldown: 0.5f,
                    travelSpeed: 10
                );
            }
            if (instance.hasShoot)
            {

                shootAttack = new Shoot(gameObject,
                    damage: new Damage(10, Damage.Type.PHYSICAL),
                    cooldown: 1f,
                    travelSpeed: 30,
                    lifetime: 2,
                    piercing: true
                );
            }
            if (instance.hasDash)
            {
                dashAttack = new Dash(gameObject,
                    damage: new Damage(10, Damage.Type.PHYSICAL),
                    cooldown: 1f,
                    travelSpeed: 0.25f, // looks like the max value before there is a pause after a dash
                    lifetime: 1f
                );
            }
            if (instance.hasSlash)
            {
                slashAttack = new Slash(gameObject,
                    damage: new Damage(10, Damage.Type.PHYSICAL),
                    cooldown: 1f,
                    travelSpeed: 10,
                    knockbackStrength: 10
                );
            }
            if (instance.hasStrafe)
            {
                strafeAttack = new Strafe(gameObject,
                    damage: new Damage(0, Damage.Type.PHYSICAL),
                    cooldown: 1f,
                    strafeStrength: 10f
                );
            }
            if (instance.hasMissile)
            {
                missileAttack = new Missile(gameObject,
                    damage: new Damage(10, Damage.Type.PHYSICAL),
                    cooldown: 1f,
                    travelSpeed: 10f,
                    piercing: false,
                    lifetime: 4f,
                    homing: true
                );
            }
        }
        PrimaryAttack = shootAttack;
        SecondaryAttack = strafeAttack;

        ChargeUpActive = UpgradeManager.Instance.chargeUpUpgradeData != null;
    }
    #endregion

    #region Input Polling
    void Update()
    {
        if (!pc.inputEnabled) {return;}
        // If chargeup upgrade is active
        if (ChargeUpActive)
        {
            // If primary attack 
            if (inputActions.Gameplay.PrimaryAttack.WasPressedThisFrame())
            {
                if (PrimaryAttack.IsReady())
                {
                    PrimaryAttack.ChargeStart = true;
                    StartCoroutine(PrimaryAttack.StartCharge());
                    return;
                }
            }

            if (inputActions.Gameplay.PrimaryAttack.WasReleasedThisFrame())
            {
                if (PrimaryAttack.ChargeStart)
                {
                    Vector3 _target = PrimaryAttack is Punch ? new Vector3(3,3) : gameObject.transform.right;
                    StartCoroutine(PrimaryAttack.EndCharge(gameObject.transform.position, _target));
                    PrimaryAttack.ChargeStart = false;
                    return;
                }
            }
        } else
        {
            if (inputActions.Gameplay.PrimaryAttack.IsPressed())
            {
                // this is how you actually attack
                if (PrimaryAttack.IsReady()) // check if in cooldown
                {
                    if (PrimaryAttack is Punch)
                    {
                        StartCoroutine(PrimaryAttack.Execute(gameObject.transform.position,
                            new Vector3(3, 3)));
                    }
                    else
                    {
                        StartCoroutine(PrimaryAttack.Execute(gameObject.transform.position,
                            gameObject.transform.right));
                    }

                }
            }
        }

        if (inputActions.Gameplay.SecondaryAttack.IsPressed() && SecondaryAttack != null)
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
                    }
                    else if (inputActions.Gameplay.Move.ReadValue<Vector2>().x > 0)
                    {
                        direction = -gameObject.transform.up;
                    }
                    if (direction == Vector3.zero) { return; }
                    StartCoroutine(SecondaryAttack.Execute(gameObject.transform.position, direction.normalized));
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

    public void resetDoubling()
    {
        slashAttack.Doubling = false; // not the best but o well
        punchAttack.Doubling = false;
        shootAttack.Doubling = false;
        missileAttack.Doubling = false;
    }


    //IEnumerator ExecuteSlashUpgrade()
    //{
    //    if (PrimaryAttack is Punch)
    //    {
    //        slashUpgradeData.IsActive = true;
    //        slashUpgradeData.LastExecute = Time.time;
    //        PrimaryAttack = slashAttack;
    //        if (doublingUpgradeData.IsActive) // check if doublingUpgrade is active
    //        {
    //            PrimaryAttack.Doubling = true;
    //        }
    //        yield return new WaitForSeconds(slashUpgradeData.Duration);
    //        while (inputActions.Gameplay.PrimaryAttack.IsPressed())
    //        {
    //            yield return null;
    //        }
    //        yield return new WaitForEndOfFrame();
    //        PrimaryAttack = punchAttack;
    //        slashUpgradeData.IsActive = false;
    //    } 
    //}
    
    //IEnumerator ExecuteMissileUpgrade()
    //{
    //    if (PrimaryAttack is Shoot)
    //    {
    //        missileUpgradeData.IsActive = true;
    //        missileUpgradeData.LastExecute = Time.time;
    //        PrimaryAttack = missileAttack;
    //        yield return new WaitForSeconds(missileUpgradeData.Duration);
    //        while (inputActions.Gameplay.PrimaryAttack.IsPressed())
    //        {
    //            yield return null;
    //        }
    //        yield return new WaitForEndOfFrame();
    //        PrimaryAttack = shootAttack;
    //        missileUpgradeData.IsActive = false;
    //    } 
    //}

    //IEnumerator ExecuteDoublingUpgrade()
    //{
    //    doublingUpgradeData.IsActive = true;
    //    doublingUpgradeData.LastExecute = Time.time;
    //    PrimaryAttack.Doubling = true;
    //    yield return new WaitForSeconds(doublingUpgradeData.Duration);
    //    slashAttack.Doubling = false; // not the best but o well
    //    punchAttack.Doubling = false;
    //    shootAttack.Doubling = false;
    //    doublingUpgradeData.IsActive = false;
    //}
}