using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Animations;

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

    private void OnDestroy()
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
        if (shipSprite.activeInHierarchy && !mechSprite.activeInHierarchy)
        {
            shipAnimator.SetTrigger("executeTransform");
        }
        else if (mechSprite.activeInHierarchy && !shipSprite.activeInHierarchy)
        {
            mechAnimator.SetTrigger("executeTransform");
        }
        
        pc.rotationLocked = true;
        yield return new WaitUntil(TransformFinished);
        pc.rotationLocked = false;

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

    bool TransformFinished()
    {
        if (shipSprite.activeInHierarchy && !mechSprite.activeInHierarchy)
        {
            return shipAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transform") && 
                   shipAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
        }
        else if (!shipSprite.activeInHierarchy && mechSprite.activeInHierarchy)
        {
            return mechAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transform") &&
                   mechAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
        }
        return false;
    }
}
