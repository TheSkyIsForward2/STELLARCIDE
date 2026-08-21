using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlashUpgrade : UpgradeData
{
    public SlashUpgrade(float duration, float cooldown) : base(duration, cooldown)
    {

    }

    public override IEnumerator Execute()
    {
        UpgradeManager upgradeManager = UpgradeManager.Instance;
        if (upgradeManager.PrimaryAttack is Punch)
        {
            upgradeManager.slashUpgradeData.IsActive = true;
            upgradeManager.slashUpgradeData.LastExecute = Time.time;
            upgradeManager.PrimaryAttack = upgradeManager.slashAttack;
            if (upgradeManager.doublingUpgradeData.IsActive) // check if doublingUpgrade is active
            {
                upgradeManager.PrimaryAttack.Doubling = true;
            }
            yield return new WaitForSeconds(upgradeManager.slashUpgradeData.Duration);
            while (inputActions.Gameplay.PrimaryAttack.IsPressed())
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            upgradeManager.PrimaryAttack = upgradeManager.punchAttack;
            upgradeManager.slashUpgradeData.IsActive = false;
        }
    }
}
