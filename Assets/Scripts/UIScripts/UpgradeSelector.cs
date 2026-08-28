using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeSelector : MonoBehaviour
{
    // References to UI card elements
    [SerializeField] Image icon;
    [SerializeField] TMPro.TextMeshProUGUI title;
    [SerializeField] TMPro.TextMeshProUGUI description;
    // References to the managers and upgrades
    UpgradeManager manager;
    UpgradeType upgrade;
    // Class to be referenced for the Upgrade Selector select button (meant to be interacted with the UpgradeManager
    // and its methods)
    private void Start()
    {
        manager = UpgradeManager.Instance;
        string name = gameObject.name;
        Debug.Log(name);
        if (int.TryParse(name, out int num)) {
            upgrade = manager.possibleUpgrades[num];
        } else
        {
            upgrade = manager.possibleUpgrades[0];
        }
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        icon.sprite = upgrade.icon;
        title.text = upgrade.upgradeName;
        description.text = upgrade.description;

    }

    // replace int with Upgrade abstract
    public void Select()
    {
        manager.ApplyUpgrade(upgrade);
        Debug.Log("applying upgrade!?");
        // Go back to game scene
        SceneManager.LoadScene(1);
    }
}
