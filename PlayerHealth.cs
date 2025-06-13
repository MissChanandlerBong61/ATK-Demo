using UnityEngine;

[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 6;
    private int currentHealth;
    private PlayerAnimationController animator;
    private Rigidbody2D rb;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<PlayerAnimationController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }

    public void TakeDamage()
    {
        animator.SetBool("Hit", true);
        rb.angularVelocity /= 2;
        Invoke(nameof(ResetHit), 0.5f);
    }

    private void ResetHit()
    {
        animator.SetBool("Hit", false);
    }
}