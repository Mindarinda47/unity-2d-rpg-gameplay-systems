using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform visual;

    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isTalking;
    [SerializeField] private PlayerState currentState;

    public bool IsDead => currentState == PlayerState.Dead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        currentState = PlayerState.Idle;
    }

    private void Update()
    {
        if (CanMove())
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            moveInput = moveInput.normalized;

            HandleFlip();
            animator.SetBool("IsMoving", moveInput != Vector2.zero);
            if (moveInput != Vector2.zero)
            {
                ChangeState(PlayerState.Move);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }
        }
    }

    private void FixedUpdate()
    {
        if (CanMove())
            rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleFlip()
    {
        if (moveInput.x > 0)
        {
            visual.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            visual.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void SetTalking(bool value)
    {
        if (IsDead)
            return;

        if (value)
        {
            moveInput = Vector2.zero;
            animator.SetBool("IsMoving", false);
            ChangeState(PlayerState.Talk);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
    }

    private void ChangeState(PlayerState newState)
    {
        if (IsDead && newState != PlayerState.Dead)
            return;

        if (currentState == newState)
            return;

        currentState = newState;

        Debug.Log($"State Changed : {currentState}");
    }

    private bool CanMove()
    {
        return currentState != PlayerState.Talk &&
               currentState != PlayerState.Dead;
    }

    private bool CanInteract()
    {
        return currentState != PlayerState.Dead;
    }

    public void SetDead()
    {
        moveInput = Vector2.zero;
        animator.SetBool("IsMoving", false);
        ChangeState(PlayerState.Dead);
    }
}
