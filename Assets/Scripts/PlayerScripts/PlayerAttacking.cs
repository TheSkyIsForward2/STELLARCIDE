using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

/* TODO:
++ store inventory of attacks & upgrades
-- i.e. AttackInventory[] & AttackPool[]
++ take attack data from attacks.json
++ initialize attacks using data
*/

/// <summary>
/// Controller script for player attacks 
/// </summary>
public class PlayerAttacking : MonoBehaviour
{
    #region Initialization
    [NonSerialized] public Attack PrimaryAttack;
    [NonSerialized] public Attack SecondaryAttack;

    [SerializeField] public GameObject chargeUpIndicator;

    private Punch punchAttack;
    private Shoot shootAttack;
    private Dash dashAttack;
    private Slash slashAttack;
    private Strafe strafeAttack;
    private Missile missileAttack;

    public struct UpgradeData
    {
        public float Cooldown;
        public float Duration;
        public float LastExecute;
        public bool IsReady(){return Cooldown + LastExecute < Time.time;}
        public UpgradeData(float duration, float cooldown)
        {
            Duration = duration;
            Cooldown = cooldown;
            LastExecute = 0;
        }
    }

    UpgradeData slashUpgradeData;
    UpgradeData missileUpgradeData;
    UpgradeData chargeUpUpgradeData;
    private PlayerControls inputActions;

    void Awake()
    {
        punchAttack = new Punch(gameObject,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 0.5f
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
            cooldown: 1f
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

        // Using negative so cause it's supposed to be passive
        chargeUpUpgradeData = new UpgradeData(
            cooldown: -10f,
            duration: 5f
        );
        inputActions = new PlayerControls();
        inputActions.Enable();

        // inputActions.Gameplay.PrimaryAttack += ()=>{};
    }

    void Start()
    {
        // this script now observes whenever the player changes forms and switches attacks accordingly
        EventBus.Instance.OnFormChange += (newMode) => SwapAttacks(newMode);
    }
    #endregion

    #region Input Polling
    // TODO: move this stuff into InputAction Events
    void Update()
    {
        // If chargeup upgrade is active
        if (true)
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
                    StartCoroutine(PrimaryAttack.EndCharge(gameObject.transform.position, gameObject.transform.right));
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

        // THESE NEED TO CHECK FOR MECH/SHIP FORM STATUS!!! ------------------------------------
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // make this check for current upgrade in Q slot and then apply!
            if (slashUpgradeData.IsReady())
                StartCoroutine(ExecuteSlashUpgrade());
        }
        
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (missileUpgradeData.IsReady())
                StartCoroutine(ExecuteMissileUpgrade());
        }
        
        // --------------------------------------------------------------------------------------
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
            slashUpgradeData.LastExecute = Time.time;
            PrimaryAttack = slashAttack;
            yield return new WaitForSeconds(slashUpgradeData.Duration);
            while (inputActions.Gameplay.PrimaryAttack.IsPressed())
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            PrimaryAttack = punchAttack;
        } 
    }
    
    IEnumerator ExecuteMissileUpgrade()
    {
        if (PrimaryAttack is Shoot)
        {
            missileUpgradeData.LastExecute = Time.time;
            PrimaryAttack = missileAttack;
            yield return new WaitForSeconds(missileUpgradeData.Duration);
            while (inputActions.Gameplay.PrimaryAttack.IsPressed())
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            PrimaryAttack = shootAttack;
        } 
    }


    // void UpgradeAttack(Func<Attack> baseAttack, Type baseAttackType, 
    //                    UpgradeData upgradeData, Attack newAttack)
    // {   
    //     Attack original = baseAttack();
    //     if (upgradeData.IsReady())
    //     {
    //         if (baseAttackType.IsInstanceOfType(original))
    //         {
    //             StartCoroutine( Swap(
    //                 baseAttack: original,
    //                 newAttack: newAttack,
    //                 setter: (a)=>original = a,
    //                 time: upgradeData.Duration
    //             ));
    //         }
    //     }
    // }

    // IEnumerator Swap(Attack baseAttack, Attack newAttack, Action<Attack> setter, float time)
    // {
    //     // Attack original = baseAttack;
    //     // baseAttack = newAttack;
    //     setter(newAttack);
    //     print(baseAttack);
    //     Debug.Log("started smart slash upgrade");
    //     yield return new WaitForSecondsRealtime(time);
    //     Debug.Log("stopped smart slash upgrade");
    //     // baseAttack = original;
    //     setter(baseAttack);
    //     print(baseAttack);
    // }
}