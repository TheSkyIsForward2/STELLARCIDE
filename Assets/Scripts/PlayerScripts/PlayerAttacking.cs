using System;
using UnityEngine;
using Unity.VisualScripting;
using System.Collections;

/* TODO:
++ store inventory of attacks
-- i.e. AttackInventory[] & AttackPool[]
++ take attack data from attacks.json
++ create attacks using data
*/

/// <summary>
/// Controller script for player attacks 
/// </summary>
public class PlayerAttacking : MonoBehaviour
{
    [NonSerialized] public Attack BaseAttack;
    [NonSerialized] public Attack BaseAttackUpgrade;
    [NonSerialized] public Attack SecondaryAttack;

    private Punch punchAttack;
    private Shoot shootAttack;
    private Dash dashAttack;
    private Slash slashAttack;

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
        BaseAttack = shootAttack;
    }

    void Start()
    {
        // this script now observes whenever the player changes forms and switches attacks accordingly
        EventBus.Instance.OnFormChange += (isShip) => SwapAttacks(isShip);
    }


    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // this is how you actually attack
            if (BaseAttack.IsReady()) // check if in cooldown
            {
                CoroutineManager.Instance.Run(BaseAttack.Execute(gameObject.transform.position, gameObject.transform.right));
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
                    CoroutineManager.Instance.Run(SecondaryAttack.Execute(gameObject.transform.position, mouseWorldPos));
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!BaseAttackUpgrade.IsUnityNull())
            {
                if (BaseAttackUpgrade.IsReady())
                {
                    
                }   
            }
        }
    }

    void SwapAttacks(bool isShip)
    {
        if (isShip)
        {
            BaseAttack = shootAttack;
            SecondaryAttack = null;
        }
        else
        {
            BaseAttack = punchAttack;
            SecondaryAttack = slashAttack;
        }  
    }
}