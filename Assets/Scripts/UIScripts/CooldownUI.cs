using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    bool cooldown = false;
    private float timer = 0;
    public KeyCode keycode = KeyCode.None;
    public Image image;
    // the cooldown time of the ability used (please grab this from the UpgradeManager)
    float cdTime = 10f;
    
    // Update is called once per frame
    void Update()
    {
        if (cooldown)
            CooldownUpdate();
        // if (Input.GetKeyDown(keycode))
        // {
        //     CooldownStart();
        // }
    }

    public void CooldownStart()
    {
        cooldown = true;
        timer = cdTime;
    }

    public void CooldownUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            image.fillAmount = timer/cdTime;
        }
        if (timer <= 0)
            CooldownEnd();
    }

    public void CooldownEnd()
    {
        cooldown = false;
        timer = 0;
    }
}
