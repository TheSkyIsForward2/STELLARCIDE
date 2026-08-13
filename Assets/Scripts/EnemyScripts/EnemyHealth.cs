using UnityEngine;

/// <summary>
/// Attach this to every enemy. This class derives from Entity but has little abstraction.
/// </summary>
public class EnemyHealth : Entity
{
    [SerializeField] private int maxHealth;
    public EnemyHealthBar  healthBar;
    
    void Start()
    {
        healthController = new HealthOwner(maxHealth, HealthOwner.Team.ENEMY, gameObject);
        Debug.Log("tf is this line doing");
        Debug.Log(healthController);
        healthBar.UpdateHealthBar(healthController.hp, maxHealth);
    }
}