using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 30;

    [Header("Identity")]
    [SerializeField] private string enemyId;

    private int currentHealth;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        Debug.Log($"{name} 피격: {currentHealth}/{maxHealth}");

        if (currentHealth == 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"{name} 격파");

        CombatEvents.RaiseEnemyKilled(enemyId);

        gameObject.SetActive(false);
    }
}