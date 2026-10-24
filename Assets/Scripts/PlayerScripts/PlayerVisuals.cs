using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ShipMovement))]
public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private GameObject shipSprite;
    private Animator shipAnimator;

    [SerializeField]  private GameObject mechSprite;
    private Animator mechAnimator;

    private PlayerController pc;


    void Awake()
    {
        pc = GetComponent<PlayerController>();
    }


    void Start()
    {
        mechAnimator = mechSprite.GetComponent<Animator>();
        shipAnimator = shipSprite.GetComponent<Animator>();

        shipSprite.SetActive(true);
        mechSprite.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Instance.OnFormChange += SwapSprites;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (EventBus.Instance != null)
        {
            EventBus.Instance.OnFormChange -= SwapSprites;
        }
    }


    void SwapSprites(PlayerMode newMode)
    {
        if (!mechSprite.activeInHierarchy && !shipSprite.activeInHierarchy)
        {
            return;
        }

        StartCoroutine(PlayTransformAnimation(newMode));
    }


    IEnumerator PlayTransformAnimation(PlayerMode newMode)
    {
        if (newMode == PlayerMode.MECH)
        {
            ForceSwapSprites(PlayerMode.SHIP);
            shipAnimator.SetTrigger("executeTransform");
        }
        else if (newMode == PlayerMode.SHIP)
        {
            ForceSwapSprites(PlayerMode.MECH);
            mechAnimator.SetTrigger("executeTransform");
        }
        
        pc.rotationLocked = true;
        yield return new WaitUntil( ()=>
        {
            return TransformFinished(newMode);
        });
        pc.rotationLocked = false;

        ForceSwapSprites(newMode);
    }

    bool TransformFinished(PlayerMode pm)
    {
        if (pm == PlayerMode.MECH)
        {
            return shipAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transform") && 
                   shipAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
        }
        else if (pm == PlayerMode.SHIP)
        {
            return mechAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transform") &&
                   mechAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
        }
        return false;
    }

    void ForceSwapSprites(PlayerMode newMode)
    {
        if (newMode == PlayerMode.MECH)
        {
            shipSprite.SetActive(false);
            mechSprite.SetActive(true);
        }
        else if (newMode == PlayerMode.SHIP)
        {
            mechSprite.SetActive(false);
            shipSprite.SetActive(true);
        }
    }
}
