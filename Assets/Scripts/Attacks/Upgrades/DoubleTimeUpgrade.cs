using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleTimeUpgrade : UpgradeData
{
    public DoubleTimeUpgrade(float duration, float cooldown) : base(duration, cooldown)
    {

    }

    public override IEnumerator Execute()
    {
        UpgradeManager upgradeManager = UpgradeManager.Instance;
        upgradeManager.doublingUpgradeData.IsActive = true;
        upgradeManager.doublingUpgradeData.LastExecute = Time.time;
        upgradeManager.PrimaryAttack.Doubling = true;
        yield return new WaitForSeconds(upgradeManager.doublingUpgradeData.Duration);
        upgradeManager.slashAttack.Doubling = false; // not the best but o well
        upgradeManager.punchAttack.Doubling = false;
        upgradeManager.shootAttack.Doubling = false;
        upgradeManager.doublingUpgradeData.IsActive = false;
    }
}
