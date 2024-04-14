using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private GroundedCheck ground;
    [SerializeField] private MovementHandler movement;
    [SerializeField] private JumpHandler jump;
    [SerializeField] private Hookshot hookshot;
    [SerializeField] private AfterimageController afterimage;
    [SerializeField] private PlayerStats playerStats;

    private bool grounded;
    private bool acting;
    private bool cancellable;
    private bool parrying;

    void Start()
    {

    }

    void FixedUpdate()
    {
        grounded = ground.CheckGrounded();

        HandleInputs();
    }

    void HandleInputs()
    {
        if (!acting && InputHandler.Instance.Move.pressed)
        {
            movement.StartAcceleration(InputHandler.Instance.Dir);
        }
        else if (!acting && InputHandler.Instance.Move.down)
        {
            movement.UpdateMovement(InputHandler.Instance.Dir);
        }
        else if (!acting)
        {
            movement.StartDeceleration();
        }

        if (((!acting && grounded) || cancellable) && InputHandler.Instance.Jump.pressed)
        {
            jump.StartJump();
        }

        if ((!acting || cancellable) && InputHandler.Instance.Attack.pressed)
        {

        }
        else if ((!acting || cancellable) && InputHandler.Instance.Grapple.pressed)
        {
            hookshot.StartHookshot();
            StartAction();
        }

        if ((!acting || cancellable) && InputHandler.Instance.Summon.pressed)
        {
            if (playerStats.TrySummon())
                StartAction();
        }
    }
    public void StartAction()
    {
        acting = true;
    }

    public void EndAction()
    {
        acting = false;
    }

    public void OnHit(HitData data)
    {
        if (parrying)
        {
            Debug.Log("lol. lmao even");
            movement.Pause(0.5f);
            jump.Pause(0.5f);
            return;
        }
        Debug.Log("Yeouch!");
        movement.Pause(0.5f);
        jump.Pause(0.5f);

        playerStats.TakeDamage(data.damage);
    }
}
