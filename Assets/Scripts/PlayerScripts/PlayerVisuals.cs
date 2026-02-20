using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ShipMovement))]
public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private GameObject shipSprite;
    [SerializeField]  private GameObject mechSprite;
    private PlayerController playerController;

    void Start() {
        playerController = GetComponent<PlayerController>();
        shipSprite.SetActive(true);
        mechSprite.SetActive(false);
        EventBus.Instance.OnFormChange += (newMode) => SwapSprites(newMode);
    }

    void SwapSprites(PlayerMode newMode)
    {
        switch (newMode)
        {
            case PlayerMode.SHIP:
                shipSprite.SetActive(true);
                mechSprite.SetActive(false);
                break;
            default:
                shipSprite.SetActive(false);
                mechSprite.SetActive(true);
                break;
        }
    }
}
