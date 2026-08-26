using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer shipSprite;
    [SerializeField] private SpriteRenderer mechSprite;

    void Start() 
    {
        shipSprite = transform.Find("ShipVisual").gameObject.GetComponent<SpriteRenderer>();
        mechSprite = transform.Find("MechVisual").gameObject.GetComponent<SpriteRenderer>();
        shipSprite.enabled = true;
        mechSprite.enabled = false;  
        EventBus.Instance.OnFormChange += (newMode) => SwapSprites(newMode);
    }

    void OnDestroy()
    {
        EventBus.Instance.OnFormChange -= (newMode) => SwapSprites(newMode);
    }

    void SwapSprites(PlayerMode newMode)
    {
        switch (newMode)
        {
            case PlayerMode.SHIP:
                shipSprite.enabled = true;
                mechSprite.enabled = false;  
                break;
            case PlayerMode.MECH:
                shipSprite.enabled = false;
                mechSprite.enabled = true;  
                break;
        }
    }
}
