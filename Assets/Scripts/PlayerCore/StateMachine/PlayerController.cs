using R3;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
    private HierarchicalStateMachine hsm;
    private Rigidbody rb;

    private IState idleState;
    private DirectionalMoveState moveState;
    private JumpState jumpState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        idleState = new IdleState(rb, animator);
        moveState = new DirectionalMoveState(rb, animator);
        jumpState = new JumpState(rb, animator, 20f, idleState);

        hsm = new HierarchicalStateMachine();
        hsm.ChangeState(idleState);

        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                Vector3 direction = Vector3.zero;

                switch (true)
                {
                    case bool _ when Input.GetKey(KeyCode.W):
                        direction = Vector3.forward;
                        break;
                    case bool _ when Input.GetKey(KeyCode.S):
                        direction = Vector3.back;
                        break;
                    case bool _ when Input.GetKey(KeyCode.A):
                        direction = Vector3.left;
                        break;
                    case bool _ when Input.GetKey(KeyCode.D):
                        direction = Vector3.right;
                        break;
                }

                bool run = Input.GetKey(KeyCode.LeftShift);

                // Movement logic
                if (direction != Vector3.zero)
                {
                    moveState.SetDirection(direction, run);
                    hsm.ChangeState(moveState);
                }
                else
                {
                    hsm.ChangeState(idleState);
                }

                // Jump logic (can stack on top of move)
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    jumpState = new JumpState(rb, animator, 5f, moveState); 
                    hsm.ChangeState(jumpState);
                }
            });
    }

    void Update()
    {
        hsm.Update();
    }
}
