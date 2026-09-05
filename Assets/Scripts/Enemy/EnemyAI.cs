using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack,
        Knockback
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackExitRange = 1.2f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private Transform target;
    private Rigidbody2D rb;
    private EnemyHealth health;
    private EnemyState currentState;

    private float nextAttackTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
            target = player.transform;
        else
            Debug.LogWarning($"{name}: Player를 찾지 못했습니다.");
    }

    private void Update()
    {
        if (target == null || health == null || health.IsDead)
            return;

        if (currentState == EnemyState.Knockback)
            return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (currentState == EnemyState.Attack)
        {
            if (distance > attackExitRange)
            {
                if (distance <= detectionRange)
                    ChangeState(EnemyState.Chase);
                else
                    ChangeState(EnemyState.Idle);
            }
        }
        else
        {
            if (distance <= attackRange)
                ChangeState(EnemyState.Attack);
            else if (distance <= detectionRange)
                ChangeState(EnemyState.Chase);
            else
                ChangeState(EnemyState.Idle);
        }

        if (currentState == EnemyState.Attack)
            TryAttack();
    }
    private void FixedUpdate()
    {
        if (target == null || health == null || health.IsDead)
            return;

        if (currentState == EnemyState.Chase)
        {
            Chase();
        }
        else if (currentState != EnemyState.Knockback)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Chase()
    {
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        Vector2 nextPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        if (target.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(attackDamage);
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        Debug.Log($"{name} 상태 변경: {currentState}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void StartKnockback()
    {
        ChangeState(EnemyState.Knockback);
    }

    public void EndKnockback()
    {
        ChangeState(EnemyState.Idle);
    }
}