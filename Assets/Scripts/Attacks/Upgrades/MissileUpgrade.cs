using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MissileUpgrade : UpgradeData
{
    public MissileUpgrade(float duration, float cooldown) : base(duration, cooldown)
    {

    }

    public override IEnumerator Execute()
    {
        UpgradeManager upgradeManager = UpgradeManager.Instance;
        if (upgradeManager.PrimaryAttack is Shoot)
        {
            upgradeManager.missileUpgradeData.IsActive = true;
            upgradeManager.missileUpgradeData.LastExecute = Time.time;
            upgradeManager.PrimaryAttack = upgradeManager.missileAttack;
            yield return new WaitForSeconds(upgradeManager.missileUpgradeData.Duration);
            while (inputActions.Gameplay.PrimaryAttack.IsPressed())
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            upgradeManager.PrimaryAttack = upgradeManager.shootAttack;
            upgradeManager.missileUpgradeData.IsActive = false;
        }
    }
}
