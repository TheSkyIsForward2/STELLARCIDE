using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ShipMovement))]
public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private GameObject shipSprite;
    [SerializeField]  private GameObject mechSprite;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        EventBus.Instance.OnFormChange += (newMode) => SwapSprites(newMode);
    }

    void Start() {
        shipSprite.SetActive(true);
        mechSprite.SetActive(false);
    }

    void SwapSprites(PlayerMode newMode)
    {
        switch (newMode)
        {
            case PlayerMode.SHIP:
                shipSprite.SetActive(true);
                mechSprite.SetActive(false);
                break;
            case PlayerMode.MECH:
                shipSprite.SetActive(false);
                mechSprite.SetActive(true);
                break;
        }
    }
}
