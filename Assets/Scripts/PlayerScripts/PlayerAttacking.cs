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

        PrimaryAttack = missileAttack;
        SecondaryAttack = strafeAttack;

        slashUpgradeData = new UpgradeData(
            cooldown: 10f,
            duration: 5f
        );

        missileUpgradeData = new UpgradeData(
            cooldown: 10f,
            duration: 5f
        );

        // What does cooldown & duration exactly mean here?
        // Also I think to have the charging up effect, we add a separate script / shape to the player
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
        // If primary attack 
        if (inputActions.Gameplay.PrimaryAttack.WasPressedThisFrame())
        {
            if (chargeUpUpgradeData.IsReady())
            {
                StartCoroutine(PrimaryAttack.StartCharge());
                return;
            }
        }

        if (inputActions.Gameplay.PrimaryAttack.WasReleasedThisFrame())
        {
            if (chargeUpUpgradeData.IsReady())
            {
                StartCoroutine(PrimaryAttack.EndCharge(gameObject.transform.position, gameObject.transform.right));
                return;
            }
        }

        return;

        //// Need to make it so the player doesn't double attack
        //if (inputActions.Gameplay.PrimaryAttack.WasPressedThisFrame())
        //{
        //    if (chargeUpUpgradeData.IsReady())
        //    {
        //        chargeUpUpgradeData.LastExecute = Time.time;
        //        chargeUpIndicator.GetComponent<SpriteRenderer>().enabled = true;
        //    }
        //}

        //if (chargeUpIndicator.GetComponent<SpriteRenderer>().enabled)
        //{
        //    chargeUpIndicator.transform.localScale += new Vector3(0.01f, 0.01f, 0);
        //    chargeUpIndicator.transform.localPosition += new Vector3(0, 0.005f, 0);
        //}

        //if (inputActions.Gameplay.PrimaryAttack.WasReleasedThisFrame())
        //{
        //    if (chargeUpUpgradeData.LastExecute != 0.0)
        //    {
        //        StartCoroutine(ExecuteChargeUpUpgrade());
        //        chargeUpIndicator.GetComponent<SpriteRenderer>().enabled = false;
        //        chargeUpIndicator.transform.localScale = new Vector3(1, 1, 1);
        //        chargeUpIndicator.transform.localPosition = new Vector3(0, 6, 0);

        //    }
        //}
        //return;

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
            PrimaryAttack = punchAttack;
        } 
    }

    IEnumerator ExecuteChargeUpUpgrade()
    {
        // I think each projectile class should have it's own create projectile that fills in the correct parameters
        // That way missiles can actually follow and whatnot
        GameManager.Instance.ProjectileManager.CreateProjectile(PrimaryAttack.Owner, 
            PrimaryAttack.Damage, PrimaryAttack.TravelSpeed, PrimaryAttack.Lifetime, PrimaryAttack.Piercing, PrimaryAttack.Homing,
            sizeScalar: 2 * (Time.time - chargeUpUpgradeData.LastExecute), gameObject.transform.position,
                        gameObject.transform.right);
        chargeUpUpgradeData.LastExecute = 0.0f;
        yield return new WaitForEndOfFrame();
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