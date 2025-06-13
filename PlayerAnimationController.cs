using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetMovement(float horizontal, float vertical)
    {
        if (Mathf.Abs(horizontal) > 0.01f)
           animator.SetFloat("Speed", Mathf.Sign(horizontal));
    }

    public void SetJumping(bool isJumping)
    {
        animator.SetBool("Jump",isJumping);
    }

    public void SetHit(bool isHit)
    {
        animator.SetBool("Hit", isHit);
    }

    public void SetFloat(string param, float value)
    {
        animator.SetFloat(param, value);
    }

    public void SetBool(string param, bool value)
    {
        animator.SetBool(param, value);
    }

    public void SetTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }
    public void ResetTrigger(string paramName)
    {
        animator.ResetTrigger(paramName);
    }
}