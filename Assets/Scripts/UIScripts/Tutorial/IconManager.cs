using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public Image spaceIcon;
    public Image qeIcon;
    public Image mouseIcon;
    public Image wasdIcon;
    public TMP_Text controlsText;
    public TMP_Text explanationText;

    // Update is called once per frame
    void Update()
    {
        if (qeIcon.enabled)
        {
            print("why");
            return;
        }

        print(playerController);
        if (playerController.GetPlayerMode() == PlayerMode.SHIP) // TODO add additional checks for progress through tutorial
        {
            wasdIcon.enabled = false;
            spaceIcon.enabled = true;
            controlsText.text = "SHIP CONTROLS";
        }
        else {
            wasdIcon.enabled = true;
            spaceIcon.enabled = false;
            controlsText.text = "MECH CONTROLS";
        }
    }

    public void Enemy()
    {
        explanationText.text = "THIS IS A STANDARD ENEMY. DEFEAT ALL ENEMIES IN AN AREA TO MOVE ON.";
        controlsText.text = "MOUSE1 TO ATTACK";
    }

    public void SwapQE()
    {
        wasdIcon.enabled = false;
        spaceIcon.enabled = false;
        mouseIcon.enabled = false;
        qeIcon.enabled = true;
    }
}
