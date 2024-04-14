using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private GroundedCheck ground;
    [SerializeField] private MovementHandler movement;
    [SerializeField] private JumpHandler jump;
    [SerializeField] private Hookshot hookshot;

    private bool grounded;

    void Start()
    {
        GameManager.Instance.CreateHitbox(new HitData())
            .Build();
    }

    void FixedUpdate()
    {
        grounded = ground.CheckGrounded();

        HandleInputs();
    }

    void HandleInputs()
    {
        var velocity = rbody.velocity;

        if (InputHandler.Instance.Move.pressed)
        {
            movement.StartAcceleration(InputHandler.Instance.Dir);
        }
        else if (InputHandler.Instance.Move.down)
        {
            movement.UpdateMovement(InputHandler.Instance.Dir);
        }
        else
        {
            movement.StartDeceleration();
        }

        if (grounded && InputHandler.Instance.Jump.pressed)
        {
            jump.StartJump();
        }

        if (InputHandler.Instance.Grapple.pressed)
        {
            hookshot.TryStartHookshot();
        }

        if (InputHandler.Instance.Attack.pressed)
        {

        }

        rbody.velocity = velocity;
    }

    public void OnHit(HitData data)
    {
        Debug.Log("Yeouch!");
        movement.Pause(0.5f);
        jump.Pause(0.5f);
    }
}
