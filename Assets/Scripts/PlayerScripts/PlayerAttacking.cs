using System;
using UnityEngine;
using Unity.VisualScripting;

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
    [NonSerialized] public Attack SecondaryAttack;
    // private GameObject self;

    void Awake()
    {
        // seems a bit redundant but for some reason this solves a bug
        // self = gameObject;
        // start off with a Shoot attack
        BaseAttack = new Shoot(gameObject,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 30,
            lifetime: 2,
            piercing: true
        );
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
                if (BaseAttack is Punch)
                    CoroutineManager.Instance.Run(BaseAttack.Execute(gameObject.transform.position, new Vector3(3,3,0)));
                else
                    CoroutineManager.Instance.Run(BaseAttack.Execute(gameObject.transform.position, gameObject.transform.right));
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (!SecondaryAttack.IsUnityNull())
            {
                if (SecondaryAttack.IsReady())
                {
                    if (SecondaryAttack is Dash)
                    {
                        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        mouseWorldPos.z = transform.position.z;
                        CoroutineManager.Instance.Run(SecondaryAttack.Execute(gameObject.transform.position, mouseWorldPos));
                    }
                    if (SecondaryAttack is Strafe)
                    {
                        Vector3 direction = Vector3.zero;
                        if (Input.GetKey(KeyCode.A)) // Strafing feels a little unintuitive right now when rotated
                        {
                            direction = gameObject.transform.up;
                        } else if (Input.GetKey(KeyCode.D))
                        {
                            direction = -gameObject.transform.up;
                        }
                        CoroutineManager.Instance.Run(SecondaryAttack.Execute(gameObject.transform.position, direction.normalized));
                    }
                }
            }
        }
    }

    void SwapAttacks(bool isShip)
    {
        if (isShip)
        {
            BaseAttack = new Shoot(gameObject,
                damage: new Damage(10, Damage.Type.PHYSICAL),
                cooldown: 1f,
                travelSpeed: 30,
                lifetime: 2,
                piercing: true
            );
            SecondaryAttack = new Strafe(gameObject,
                damage: new Damage(10, Damage.Type.PHYSICAL),
                cooldown: 5f,
                strafeStrength: 10f
            );
        }
        else
        {
            BaseAttack = new Punch(gameObject,
                damage: new Damage(10, Damage.Type.PHYSICAL),
                cooldown: 0.5f
            );

            SecondaryAttack = new Dash(gameObject, 
                damage: new Damage(10, Damage.Type.PHYSICAL),
                cooldown: 1f,
                travelSpeed:0.25f, // looks like the max value before there is a pause after a dash
                lifetime: 1f
            );
        }  
    }
}