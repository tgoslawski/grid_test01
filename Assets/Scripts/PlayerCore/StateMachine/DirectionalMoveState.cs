using UnityEngine;

public class DirectionalMoveState : BaseState
{
    private Rigidbody rb;
    private Vector3 moveDirection;
    private float walkSpeed;
    private float runSpeed;
    private bool isRunning;
    private Animator animator;

    public DirectionalMoveState(Rigidbody rb, Animator animator, float walkSpeed = 3f, float runSpeed = 6f)
    {
        this.rb = rb;
        this.walkSpeed = walkSpeed;
        this.animator = animator;
        this.runSpeed = runSpeed;
        moveDirection = Vector3.zero;
    }

    public void SetDirection(Vector3 direction, bool run = false)
    {
        moveDirection = direction.normalized;
        isRunning = run;
    }

    public override void Execute()
    {
        float speed = isRunning ? runSpeed : walkSpeed;
        Vector3 targetVelocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);

        float controlFactor = IsGrounded() ? 1f : 0.5f; // less control in air
        rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, 0.1f * controlFactor);

        if (moveDirection.sqrMagnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
    
    private bool IsGrounded()
    {
        return Physics.Raycast(rb.position, Vector3.down, 1.1f);
    }
}