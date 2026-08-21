using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

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

    private PlayerControls inputActions;

    void Awake()
    {
        UpgradeManager.Instance.CreateAttacks(gameObject);
        PrimaryAttack = UpgradeManager.Instance.shootAttack;
        SecondaryAttack = null;

        inputActions = new PlayerControls();
        inputActions.Enable();

        // Primary Upgrades
        inputActions.Gameplay.UpgradeA.performed += (ctx) =>
        {
            if (UpgradeManager.Instance.PrimaryUpgrade == null) { return;  }
            if (UpgradeManager.Instance.PrimaryUpgrade.IsReady())
            {
                UpgradeManager.Instance.PrimaryUpgrade.Execute();
            }
        };
        
        inputActions.Gameplay.UpgradeB.performed += (ctx) =>
        {
            if (UpgradeManager.Instance.SecondaryUpgrade == null) { return; }
            if (UpgradeManager.Instance.SecondaryUpgrade.IsReady())
            {
                UpgradeManager.Instance.SecondaryUpgrade.Execute();
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
        // If chargeup upgrade is active
        if (UpgradeManager.Instance.chargeUpUpgradeData != null)
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
                PrimaryAttack = UpgradeManager.Instance.shootAttack;
                SecondaryAttack = UpgradeManager.Instance.strafeAttack;
                break;
            default:
                PrimaryAttack = UpgradeManager.Instance.punchAttack;
                SecondaryAttack = UpgradeManager.Instance.dashAttack;
                break;
        }
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