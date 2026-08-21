using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UpgradeTarget
{
    SLASH,
    STRAFE,
    MISSILE,
    CHARGE_UP,
    DOUBLE_TIME
}

public enum UpgradeEffect
{
    CREATE,
    COOLDOWN_MINUS,
    DURATION_PLUS
}
public struct UpgradeType
{
    public string upgradeName;
    public string description;
    public Sprite icon;

    public UpgradeTarget upgradeTarget;
    public UpgradeEffect upgradeEffect;

    public UpgradeType(string name, string desc, UpgradeTarget target, UpgradeEffect effect)
    {
        upgradeName = name;
        description = desc;
        upgradeTarget = target;
        upgradeEffect = effect;

        switch (target)
        {
            case UpgradeTarget.SLASH:
                icon = null;
                break;
            // Add the rest once we get assets in
            default:
                icon = null;
                break;
        }
    }
    public UpgradeType(string name, string desc, Sprite img, UpgradeTarget target, UpgradeEffect effect)
    {
        upgradeName = name;
        description = desc;
        upgradeTarget = target;
        upgradeEffect = effect;
        icon = img;
    }
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public Punch punchAttack;
    public Shoot shootAttack;
    public Dash dashAttack;
    public Slash slashAttack;
    public Strafe strafeAttack;
    public Missile missileAttack;

    public UpgradeData slashUpgradeData;
    public UpgradeData missileUpgradeData;
    public UpgradeData chargeUpUpgradeData;
    public UpgradeData doublingUpgradeData;

    public List<UpgradeType> possibleUpgrades = new List<UpgradeType>();

    public Attack PrimaryAttack;
    public Attack SecondaryAttack;

    public UpgradeData PrimaryUpgrade;
    public UpgradeData SecondaryUpgrade;
    public void CreateAttacks(GameObject player)
    {
        punchAttack = new Punch(player,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 0.5f,
            travelSpeed: 10
        );
        shootAttack = new Shoot(player,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 30,
            lifetime: 2,
            piercing: true
        );
        dashAttack = new Dash(player,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 0.25f, // looks like the max value before there is a pause after a dash
            lifetime: 1f
        );
        slashAttack = new Slash(player,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 10
        );
        missileAttack = new Missile(player,
            damage: new Damage(10, Damage.Type.PHYSICAL),
            cooldown: 1f,
            travelSpeed: 10f,
            piercing: false,
            lifetime: 4f,
            homing: true
        );
    }
    void Awake()
    {
        Instance = this;
        // Maybe load this from data?  I'm an idiot man I forgot about that
        possibleUpgrades.Add(new UpgradeType("Slash", "it slashes", UpgradeTarget.SLASH, UpgradeEffect.CREATE));
        possibleUpgrades.Add(new UpgradeType("Missile", "it slashes", UpgradeTarget.MISSILE, UpgradeEffect.CREATE));
        possibleUpgrades.Add(new UpgradeType("Strafe", "it slashes", UpgradeTarget.STRAFE, UpgradeEffect.CREATE));
        possibleUpgrades.Add(new UpgradeType("Charge Up", "it slashes", UpgradeTarget.SLASH, UpgradeEffect.CREATE));
        possibleUpgrades.Add(new UpgradeType("Double Time", "it slashes", UpgradeTarget.SLASH, UpgradeEffect.CREATE));
    }

    public void ApplyUpgrade(UpgradeType upgrade)
    {
        switch (upgrade.upgradeEffect)
        {
            case UpgradeEffect.CREATE:
                UpgradeData createdUpgrade = null;
                switch (upgrade.upgradeTarget)
                {
                    case UpgradeTarget.STRAFE:
                        strafeAttack = new Strafe(GameObject.FindWithTag("Player"),
                            damage: new Damage(10, Damage.Type.PHYSICAL),
                            cooldown: 3f,
                            strafeStrength: 10f
                        );
                        break;
                    case UpgradeTarget.SLASH:
                        slashAttack = new Slash(GameObject.FindWithTag("Player"),
                            damage: new Damage(10, Damage.Type.PHYSICAL),
                            cooldown: 1f,
                            travelSpeed: 10
                        );
                        slashUpgradeData = new SlashUpgrade(
                            cooldown: 10f,
                            duration: 5f
                        );
                        createdUpgrade = slashUpgradeData;
                        break;
                    case UpgradeTarget.MISSILE:
                        missileAttack = new Missile(GameObject.FindWithTag("Player"),
                            damage: new Damage(10, Damage.Type.PHYSICAL),
                            cooldown: 1f,
                            travelSpeed: 10f,
                            piercing: false,
                            lifetime: 4f,
                            homing: true
                        );
                        missileUpgradeData = new MissileUpgrade(
                            cooldown: 10f,
                            duration: 5f
                        );
                        createdUpgrade = missileUpgradeData;
                        break;
                    case UpgradeTarget.DOUBLE_TIME:
                        doublingUpgradeData = new DoubleTimeUpgrade(
                            cooldown: 10f,
                            duration: 5f
                        );
                        createdUpgrade = doublingUpgradeData;
                        break;
                    case UpgradeTarget.CHARGE_UP:
                        chargeUpUpgradeData = new ChargeUpUpgrade(
                            cooldown: 10f,
                            duration: 5f
                        );
                        createdUpgrade = chargeUpUpgradeData;
                        break;
                }

                possibleUpgrades.Remove(upgrade);
                possibleUpgrades.Add(new UpgradeType(upgrade.upgradeName, upgrade.description,
                    upgrade.icon, upgrade.upgradeTarget, UpgradeEffect.COOLDOWN_MINUS));
                possibleUpgrades.Add(new UpgradeType(upgrade.upgradeName, upgrade.description,
                    upgrade.icon, upgrade.upgradeTarget, UpgradeEffect.DURATION_PLUS));
                if (PrimaryUpgrade == null)
                {
                    PrimaryUpgrade = createdUpgrade;
                } else
                {
                    SecondaryUpgrade = createdUpgrade;
                    removeCreateUpgrades();
                }
                break;
            case UpgradeEffect.DURATION_PLUS:
                switch (upgrade.upgradeTarget)
                {
                    case UpgradeTarget.STRAFE:
                        strafeAttack.StrafeStrength += 5;
                        if (strafeAttack.StrafeStrength >= 20)
                        {
                            possibleUpgrades.Remove(upgrade);
                        }
                        break;
                    case UpgradeTarget.SLASH:
                        slashUpgradeData.Duration += 0.5f;
                        if (slashUpgradeData.Duration == slashUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                    case UpgradeTarget.MISSILE:
                        missileUpgradeData.Duration += 0.5f;
                        if (missileUpgradeData.Duration == missileUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                    case UpgradeTarget.DOUBLE_TIME:
                        doublingUpgradeData.Duration += 0.5f;
                        if (doublingUpgradeData.Duration == doublingUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                    case UpgradeTarget.CHARGE_UP:
                        chargeUpUpgradeData.Duration += 0.5f;
                        if (chargeUpUpgradeData.Duration == chargeUpUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                }
                break;
            case UpgradeEffect.COOLDOWN_MINUS:
                switch (upgrade.upgradeTarget)
                {
                    case UpgradeTarget.STRAFE:
                        strafeAttack.Cooldown -= 1;
                        if (strafeAttack.Cooldown <= 1)
                        {
                            possibleUpgrades.Remove(upgrade);
                        }
                        break;
                    case UpgradeTarget.SLASH:
                        slashUpgradeData.Cooldown -= 0.5f;
                        if (slashUpgradeData.Duration == slashUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                    case UpgradeTarget.MISSILE:
                        missileUpgradeData.Cooldown -= 0.5f;
                        if (missileUpgradeData.Duration == slashUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                    case UpgradeTarget.DOUBLE_TIME:
                        doublingUpgradeData.Cooldown -= 0.5f;
                        if (doublingUpgradeData.Duration == doublingUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                    case UpgradeTarget.CHARGE_UP:
                        chargeUpUpgradeData.Cooldown -= 0.5f;
                        if (chargeUpUpgradeData.Duration == chargeUpUpgradeData.Cooldown)
                        {
                            removeDurationAndCooldown(upgrade);
                        }
                        break;
                }
                break;
        }
    }

    public void removeDurationAndCooldown(UpgradeType upgrade)
    {
        possibleUpgrades.RemoveAll(up =>
            up.upgradeTarget == upgrade.upgradeTarget &&
            up.upgradeEffect != UpgradeEffect.CREATE
        );
    }

    private void removeCreateUpgrades()
    {
        possibleUpgrades.RemoveAll(upgrade =>
            upgrade.upgradeEffect == UpgradeEffect.CREATE &&
            upgrade.upgradeTarget != UpgradeTarget.STRAFE
        );
    }
}
