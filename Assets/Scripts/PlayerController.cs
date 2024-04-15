using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GroundedCheck ground;
    [SerializeField] private MovementHandler movement;
    [SerializeField] private JumpHandler jump;
    [SerializeField] private Hookshot hookshot;
    [SerializeField] private AfterimageController afterimage;
    [SerializeField] private PlayerStats playerStats;

    private Interactable focusedInteract;
    private List<Interactable> interactables = new();
    private bool grounded;
    private bool acting;
    private bool cancellable;
    private bool parrying;

    void Start()
    {

    }

    void FixedUpdate()
    {
        UpdateInteractables();

        grounded = ground.CheckGrounded();
        animator.SetBool("grounded", grounded);

        HandleInputs();
    }

    void HandleInputs()
    {
        if (!acting && InputHandler.Instance.Move.pressed)
        {
            movement.StartAcceleration(InputHandler.Instance.Dir);
            animator.SetBool("running", true);
            if (InputHandler.Instance.Dir != 0)
                sprite.flipX = InputHandler.Instance.Dir < 0;
        }
        else if (!acting && InputHandler.Instance.Move.down)
        {
            movement.UpdateMovement(InputHandler.Instance.Dir);
            animator.SetBool("running", true);
            if (InputHandler.Instance.Dir != 0)
                sprite.flipX = InputHandler.Instance.Dir < 0;
        }
        else if (!acting)
        {
            animator.SetBool("running", false);
            movement.StartDeceleration();
        }

        if (((!acting && grounded) || cancellable) && InputHandler.Instance.Jump.pressed)
        {
            jump.StartJump();
        }

        if (focusedInteract != null && InputHandler.Instance.Interact.pressed)
        {
            focusedInteract.Interact();
        }

        if ((!acting || cancellable) && InputHandler.Instance.Attack.pressed)
        {
            StartAction();
            movement.StartDeceleration();
            animator.SetTrigger("attack");
            Vector2 toMouse = ((Vector2)(Camera.main.ScreenToWorldPoint(InputHandler.Instance.MousePos) - transform.position)).normalized;
            sprite.flipX = toMouse.x < 0;

        }
        else if ((!acting || cancellable) && InputHandler.Instance.Grapple.pressed)
        {
            StartAction();
            

            Vector2 toMouse = Utils.GetMouseDir(transform.position);
            sprite.flipX = toMouse.x < 0;

            hookshot.StartHookshot();
        }
        else if ((!acting || cancellable) && InputHandler.Instance.Summon.pressed)
        {
            StartAction();
            playerStats.TrySummon();
        }
    }
    public void StartAction()
    {
        animator.SetBool("running", false);
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

    public void AddInteractable(Interactable interact)
    {
        interactables.Add(interact);
    }

    public void RemoveInteractable(Interactable interact)
    {
        interactables.Remove(interact);
        if (interact == focusedInteract)
        {
            focusedInteract = null;
            interact?.Unfocus();
        }
    }

    public void UpdateInteractables()
    {
        Interactable newFocus = null;
        foreach (var interact in interactables)
        {
            if (newFocus == null || Vector2.Distance(transform.position, newFocus.transform.position) > Vector2.Distance(transform.position, interact.transform.position))
                newFocus = interact;
        }

        if (focusedInteract != newFocus)
        {
            focusedInteract?.Unfocus();
            focusedInteract = newFocus;
            focusedInteract?.Focus();
        }
    }

    public void FireAttack() {
        var toMouse = Utils.GetMouseDir(transform.position);

        GameManager.Instance.CreateHitbox(new HitData() {damage = 1})
            .SetPos((Vector2)transform.position + 3 * toMouse)
            .SetAnimation(HitboxAnim.PLAYER_SWORD)
            .SetRotation(Quaternion.FromToRotation(Vector2.right, toMouse).eulerAngles)
            .SetTeam(HitboxTeam.PLAYER)
            .SetSize(new(6, 2))
            .SetDuration(HitboxData.MATCH_ANIM_DURATION)
            .Build();

    }
}
