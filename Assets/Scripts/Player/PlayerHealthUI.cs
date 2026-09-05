using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth가 연결되지 않았습니다.");
            return;
        }

        playerHealth.HealthChanged += HandleHealthChanged;

        Refresh(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.HealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        Refresh(currentHealth, maxHealth);
    }

    private void Refresh(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        healthText.text = $"{currentHealth} / {maxHealth}";
    }
}