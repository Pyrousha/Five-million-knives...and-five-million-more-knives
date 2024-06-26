using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    [SerializeField] private GameObject summonPrefab;

    private Interactable focusedInteract;
    private List<Interactable> interactables = new();
    private bool grounded;
    private bool acting;
    private bool cancellable;
    private bool parrying;
    private float pauseUntil;

    private WeaponType currentWeapon = WeaponType.SWORD;

    void Awake()
    {
        GameManager.Instance.RegisterPlayer(this);
    }

    void FixedUpdate()
    {
        UpdateInteractables();

        grounded = ground.CheckGrounded();
        animator.SetBool("grounded", grounded);

        if (Time.time > pauseUntil) {
            animator.speed = 1;
        }
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
            if (cancellable) {
                UnpausePlayer();
                cancellable = false;
                acting = false;
                parrying = false;
            }

            animator.SetBool("running", true);
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

            animator.SetTrigger("grapple");
            parrying = true;
        }
        else if ((!acting || cancellable) && InputHandler.Instance.Summon.pressed && playerStats.TryConsumeStamina(PlayerStats.SUMMON_COST))
        {
            //StartAction();
            if (cancellable) {
                UnpausePlayer();
                cancellable = false;
                acting = false;
                parrying = false;
            }
            animator.SetBool("running", true);
            Instantiate(summonPrefab, new Vector2(Random.Range(-10, 0), Random.Range(4, 6)), Quaternion.identity);
            Instantiate(summonPrefab, new Vector2(Random.Range(0, 11), Random.Range(4, 6)), Quaternion.identity);
        }
    }
    public void StartAction()
    {
        if (cancellable) {
            UnpausePlayer();
            cancellable = false;
        }
        parrying = false;
        animator.SetBool("running", false);
        acting = true;
    }

    public void EndAction()
    {
        acting = false;
        parrying = false;
    }

    public void OnHit(HitData data)
    {
        if (parrying)
        {
            Debug.Log("lol. lmao even");
            PausePlayer(0.25f);
            cancellable = true;
            return;
        }
        Debug.Log("Yeouch!");
        PausePlayer(0.25f);

        playerStats.TakeDamage(data.damage);
    }

    public void PausePlayer(float time) {
        movement.Pause(time);
        jump.Pause(time);
        animator.speed = 0;
        pauseUntil = Time.time + time;
    }

    public void UnpausePlayer() {
        movement.Pause(-1);
        jump.Pause(-1);
        animator.speed = 1;
        pauseUntil = 0;
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

    public void FireAttack()
    {
        var toMouse = Utils.GetMouseDir(transform.position);


        switch (currentWeapon)
        {
            case WeaponType.SWORD:
                GameManager.Instance.CreateHitbox(new HitData() { damage = 1 })
                    .SetPos((Vector2)transform.position + 3 * toMouse)
                    .SetAnimation(HitboxAnim.PLAYER_SWORD)
                    .SetRotation(Quaternion.FromToRotation(Vector2.right, toMouse).eulerAngles)
                    .SetTeam(HitboxTeam.PLAYER)
                    .SetSize(new(6, 2))
                    .SetDuration(HitboxData.MATCH_ANIM_DURATION)
                    .Build();
                break;
            case WeaponType.ARROW:
                GameManager.Instance.CreateHitbox(new HitData() { damage = 1 })
                    .SetPos((Vector2)transform.position + 1 * toMouse)
                    .SetAnimation(HitboxAnim.PLAYER_ARROW)
                    .SetRotation(Quaternion.FromToRotation(Vector2.right, toMouse).eulerAngles)
                    .SetTeam(HitboxTeam.PLAYER)
                    .SetSize(new(1, 1))
                    .SetDuration(0.5f)
                    .SetVelocity(30 * toMouse)
                    .Build();
                GameManager.Instance.CreateHitbox(new HitData() { damage = 1 })
                    .SetPos((Vector2)transform.position + 1 * toMouse)
                    .SetAnimation(HitboxAnim.PLAYER_ARROW)
                    .SetRotation(Quaternion.FromToRotation(Vector2.right, toMouse).eulerAngles + 10 * Vector3.forward)
                    .SetTeam(HitboxTeam.PLAYER)
                    .SetSize(new(1, 1))
                    .SetDuration(0.5f)
                    .SetVelocity(Quaternion.Euler(Quaternion.FromToRotation(Vector2.right, toMouse).eulerAngles + 10 * Vector3.forward) * (30 * Vector2.right))
                    .Build();
                GameManager.Instance.CreateHitbox(new HitData() { damage = 1 })
                    .SetPos((Vector2)transform.position + 1 * toMouse)
                    .SetAnimation(HitboxAnim.PLAYER_ARROW)
                    .SetRotation(Quaternion.FromToRotation(Vector2.right, toMouse).eulerAngles - 10 * Vector3.forward)
                    .SetTeam(HitboxTeam.PLAYER)
                    .SetSize(new(1, 1))
                    .SetDuration(0.5f)
                    .SetVelocity(Quaternion.Euler(Quaternion.FromToRotation(Vector2.right, toMouse).eulerAngles - 10 * Vector3.forward) * (30 * Vector2.right))
                    .Build();
                break;
        }
    }

    public void FireGrapple() {
        parrying = false;
        hookshot.StartHookshot();
    }

    public void PickupWeapon(WeaponType type) {
        this.currentWeapon = type;
        playerStats.GainSP(0.5f);
    }

    public void EndGrapple() {
        animator.SetTrigger("end_grapple");
    }
}

public enum WeaponType
{
    NONE, SWORD, ARROW, MAX
}
