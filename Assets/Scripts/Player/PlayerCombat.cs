using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 0.7f;
    [SerializeField] private float attackCooldown = 0.5f;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float knockbackForce = 3f;

    private float nextAttackTime;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (playerHealth == null || playerHealth.IsDead)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(attackDamage);

            if (hit.TryGetComponent(out EnemyKnockback knockback))
            {
                Vector2 direction = ((Vector2)hit.transform.position -
                                     (Vector2)transform.position).normalized;

                knockback.ApplyKnockback(direction, knockbackForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}