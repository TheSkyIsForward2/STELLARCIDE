using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleTimeUpgrade : UpgradeData
{
    public DoubleTimeUpgrade(float duration, float cooldown) : base(duration, cooldown)
    {

    }

    public override IEnumerator Execute(PlayerAttacking player)
    {
        UpgradeManager upgradeManager = UpgradeManager.Instance;
        upgradeManager.doublingUpgradeData.IsActive = true;
        upgradeManager.doublingUpgradeData.LastExecute = Time.time;
        player.PrimaryAttack.Doubling = true;
        yield return new WaitForSeconds(upgradeManager.doublingUpgradeData.Duration);
        player.resetDoubling();
        upgradeManager.doublingUpgradeData.IsActive = false;
    }
}
