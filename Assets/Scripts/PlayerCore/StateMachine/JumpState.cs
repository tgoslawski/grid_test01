using UnityEngine;

public class JumpState : BaseState
{
    private Rigidbody rb;
    private float jumpForce;
    private IState fallbackState; // where to go after landing
    private bool jumped;
    private Animator animator;

    public JumpState(Rigidbody rb, Animator animator, float jumpForce = 5f, IState fallbackState = null)
    {
        this.rb = rb;
        this.jumpForce = jumpForce;
        this.fallbackState = fallbackState;
    }

    public override void Enter()
    {
        base.Enter();

        if (!jumped)
        {
            // Keep horizontal velocity, reset vertical only
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumped = true;
        }
    }

    public override void Execute()
    {
        if (IsGrounded() && rb.velocity.y <= 0.01f)
        {
            jumped = false;
            if (fallbackState != null)
                fallbackState.Enter();
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(rb.position, Vector3.down, 1.1f);
    }
}