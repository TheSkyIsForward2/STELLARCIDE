using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [NonSerialized] public HealthOwner health;
    private Slider healthBar;
    [SerializeField] private TextMeshProUGUI numberLabel;

    void Awake()
    {
        //maxHealth = playerHealth.healthController.maxHP;
        health = GameManager.Instance.Player.GetComponent<PlayerHealth>().healthController;
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = health.maxHP;
        numberLabel.text = $"{health.hp} / {health.maxHP}";
    }

    void Update()
    {
        healthBar.value = (float) health.hp;
        numberLabel.text = $"{health.hp} / {health.maxHP}";
    }
}