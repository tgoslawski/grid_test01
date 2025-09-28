using UnityEngine;
using UnityEngine.EventSystems;

public class IdleState : BaseState
{
    private Rigidbody rb;
    private Animator animator;

    public IdleState(Rigidbody rb, Animator animator)
    {
        this.animator = animator;
        this.rb = rb;
    }

    public override void Enter()
    {
        base.Enter();
        animator.SetTrigger("Idle");
        rb.velocity = Vector3.zero; // Stop movement
    }

    public override void Execute()
    {
        
    }
}

public class MoveState : BaseState
{
    protected Rigidbody rb;
    protected Animator animator;
    protected float speed = 3f;

    public MoveState(Rigidbody rb, Animator animator)
    {
        this.rb = rb;
    }

    public override void Execute()
    {
        animator.SetTrigger("Walk");
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0, v).normalized;

        rb.velocity = direction * speed;
        
    }
}

public class RunState : MoveState
{
    public RunState(Rigidbody rb, Animator animator) : base(rb, animator)
    {
        speed = 6f; // Faster than walking
    }

    public override void Enter()
    {
        base.Enter();
        animator.SetTrigger("Run");
        Debug.Log("Running!");
    }

    public override void Execute()
    {
        base.Execute();
    }
}