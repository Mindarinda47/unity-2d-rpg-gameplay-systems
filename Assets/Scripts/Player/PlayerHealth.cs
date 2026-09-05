using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"Player 피격: {currentHealth}/{maxHealth}");

        if (currentHealth == 0)
            Die();
    }

    public int Heal(int amount)
    {
        if (isDead || amount <= 0 || currentHealth >= maxHealth)
            return 0;

        int previousHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        int healedAmount = currentHealth - previousHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"HP 회복: +{healedAmount} / {currentHealth}/{maxHealth}");

        return healedAmount;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Player 사망");

        Died?.Invoke();

        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (movement != null)
            movement.SetDead();
    }

#if UNITY_EDITOR
    [ContextMenu("Test/Take 30 Damage")]
    private void TestTakeDamage()
    {
        TakeDamage(30);
    }
#endif
}