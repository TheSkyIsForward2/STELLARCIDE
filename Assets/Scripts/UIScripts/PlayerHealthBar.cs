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
        health = GameManager.Instance.Player.GetComponent<PlayerHealth>().healthController;
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = health.maxHP;
    }

    void Start()
    {
        // numberLabel.text = $"{health.hp} / {health.maxHP}";
        numberLabel.SetText($"{health.hp} / {health.maxHP}");
    }

    void Update()
    {
        healthBar.value = (float) health.hp;
        // numberLabel.text = $"{health.hp} / {health.maxHP}";
        numberLabel.SetText($"{health.hp} / {health.maxHP}");
    }
}