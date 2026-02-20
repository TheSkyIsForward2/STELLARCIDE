using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HealthOwner health;
    
    [Header("References")]
    [SerializeField] private Slider healthBar;
    //[SerializeField] private PlayerHealth playerHealth;

    void Awake()
    {
        //maxHealth = playerHealth.healthController.maxHP;
        health = GameManager.Instance.Player.GetComponent<PlayerHealth>().healthController;
        healthBar.maxValue = health.maxHP;
    }
}