using System.Collections;
using UnityEngine;

public class EnemyKnockback : MonoBehaviour
{
    [SerializeField] private float knockbackDuration = 0.15f;

    private Rigidbody2D rb;
    private EnemyAI enemyAI;
    private Coroutine knockbackCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(
            KnockbackRoutine(direction.normalized, force)
        );
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force)
    {
        enemyAI.StartKnockback();

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        rb.linearVelocity = Vector2.zero;
        enemyAI.EndKnockback();

        knockbackCoroutine = null;
    }
}