using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;

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

    private Punch punchAttack;
    private Shoot shootAttack;
    private Dash dashAttack;
    private Slash slashAttack;

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
    UpgradeData chargeUpUpgrdeData;

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
        PrimaryAttack = shootAttack;

        slashUpgradeData = new UpgradeData(
            cooldown: 10f,
            duration: 5f
        );
    }

    void Start()
    {
        // this script now observes whenever the player changes forms and switches attacks accordingly
        EventBus.Instance.OnFormChange += (isShip) => SwapAttacks(isShip);
    }
    #endregion

    #region Input Polling
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // this is how you actually attack
            if (PrimaryAttack.IsReady()) // check if in cooldown
            {
                StartCoroutine(PrimaryAttack.Execute(gameObject.transform.position, 
                    gameObject.transform.right));
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (!SecondaryAttack.IsUnityNull())
            {
                if (SecondaryAttack.IsReady())
                {
                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    mouseWorldPos.z = transform.position.z;
                    StartCoroutine(SecondaryAttack.Execute(gameObject.transform.position, mouseWorldPos));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (slashUpgradeData.IsReady())
                StartCoroutine(ExecuteSlashUpgrade());
        }

        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     UpgradeAttack(()=>PrimaryAttack, typeof(Punch), slashUpgradeData, slashAttack);
        // }
    }
    #endregion

    void SwapAttacks(bool isShip)
    {
        if (isShip)
        {
            PrimaryAttack = shootAttack;
            SecondaryAttack = null;
        }
        else
        {
            PrimaryAttack = punchAttack;
            SecondaryAttack = slashAttack;
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