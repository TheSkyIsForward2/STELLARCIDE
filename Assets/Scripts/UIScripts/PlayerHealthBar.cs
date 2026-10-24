using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private HealthOwner health;
    private Slider healthBar;
    [SerializeField] private TextMeshProUGUI numberLabel;

    [Header("Shake Animation")]
    public float shakeDuration = 0.25f;
    public float shakeStrength = 8;
    public AnimationCurve shakeEasing;

    void Awake()
    {
        healthBar = GetComponent<Slider>();
        GameManager.Instance.healthBar = this;
    }

    void Start()
    {
        health = GameManager.Instance.Player.GetComponent<PlayerHealth>().healthController;
        healthBar.maxValue = health.maxHP;
        numberLabel.SetText($"{health.hp} / {health.maxHP}");
    }

    void OnEnable()
    {
        EventBus.Instance.OnPlayerHit += UpdateHPBar;
    }

    void OnDisable()
    {
        EventBus.Instance.OnPlayerHit -= UpdateHPBar;
    }

    void UpdateHPBar()
    {
        UpdateHPValues();
        StartCoroutine(ShakeHPBar());
    }

    void UpdateHPValues()
    {
        healthBar.value = health.hp;
        numberLabel.SetText($"{health.hp} / {health.maxHP}");
    }

    IEnumerator ShakeHPBar()
    {
        float elapsedTime = 0f;

        while ( elapsedTime  < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = shakeEasing.Evaluate(elapsedTime/shakeDuration);
            float zRotation = Mathf.Lerp(0, shakeStrength, t) % 360f;
            transform.eulerAngles = new Vector3(0, 0, zRotation);
            yield return null;
        }
        transform.eulerAngles = Vector3.zero;
    }
}