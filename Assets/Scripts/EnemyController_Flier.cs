using UnityEngine;

public class EnemyController_Flier : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyStats stats;
    [SerializeField] private SpriteRenderer sprRend;
    [SerializeField] private Animator anim;

    [Space(10)]

    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float frictionSpeed;
    [SerializeField] private float targDistToPlayer;
    [SerializeField] private float spinRadius;

    [SerializeField] private float attackCooldown;
    private float currAttackCooldown;

    private static float attackWaitTime = 0.25f;
    private Vector2 toPlayer;

    private Transform player;

    private bool actionable;
    private float endStun;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStats>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayer().transform;

        actionable = true;

        EnemySpawner.Instance.OnEnemySpawned();

        currAttackCooldown = attackCooldown;
    }

    private void FixedUpdate()
    {
        Vector2 velocity_local_friction = rb.velocity;

        velocity_local_friction = velocity_local_friction.normalized * Mathf.Max(0, velocity_local_friction.magnitude - frictionSpeed);
        Vector2 updatedVelocity = velocity_local_friction;

        if (Time.time >= endStun)
        {
            anim.SetBool("IsBeingAttacked", false);

            if (actionable && !stats.IsDead)
            {
                toPlayer = (player.position - transform.position);

                float targToPlayer = toPlayer.magnitude - targDistToPlayer;
                Vector2 targDir = toPlayer.normalized * targToPlayer;
                targDir += new Vector2(Mathf.Sin(Time.time), Mathf.Cos(Time.time)) * spinRadius;

                Vector2 velocity_with_input = velocity_local_friction + targDir * accelSpeed;

                if (velocity_local_friction.magnitude <= maxMoveSpeed)
                {
                    //under max speed, accelerate towards max speed
                    updatedVelocity = velocity_with_input.normalized * Mathf.Min(maxMoveSpeed, velocity_with_input.magnitude);
                }
                else
                {
                    float velocityOntoInput = Vector3.Project(velocity_with_input, targDir).magnitude;
                    if (Vector3.Dot(velocity_with_input, targDir) < 0)
                        velocityOntoInput *= -1;

                    //Debug.Log(velocityOntoInput);
                    if (velocityOntoInput <= maxMoveSpeed)
                    {
                        //Speed in direction of input lower than maxSpeed
                        updatedVelocity = velocity_with_input;
                    }
                    else
                    {
                        //Would accelerate more, so don't user player input directly

                        Vector3 velocityOntoFriction = Vector3.Project(velocity_local_friction, targDir);

                        Vector3 perp = (Vector3)velocity_local_friction - velocityOntoFriction;

                        //Accelerate towards max speed
                        float amountToAdd = Mathf.Max(0, Mathf.Min(maxMoveSpeed - velocityOntoFriction.magnitude, targDir.magnitude));
                        float perpAmountToSubtract = Mathf.Max(0, Mathf.Min(accelSpeed - amountToAdd, perp.magnitude));

                        perp = perp.normalized * perpAmountToSubtract;

                        updatedVelocity = (velocity_local_friction + amountToAdd * targDir.normalized - (Vector2)perp);
                    }
                }

                currAttackCooldown -= Time.fixedDeltaTime;
                if (currAttackCooldown <= 0)
                {
                    anim.SetTrigger("Attack");

                    actionable = false;

                    currAttackCooldown = attackCooldown - attackWaitTime;
                }
            }
        }

        rb.velocity = updatedVelocity;

        if (rb.velocity.x > 0)
            sprRend.flipX = false;
        else if (rb.velocity.x < 0)
            sprRend.flipX = true;
    }

    public void FireAttack()
    {
        toPlayer.Normalize();

        //Attack player
        GameManager.Instance.CreateHitbox(new HitData() { damage = 1 })
        .SetPos((Vector2)transform.position + 1 * toPlayer)
        .SetAnimation(HitboxAnim.ENEMY_PROJECTILE)
        .SetRotation(new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(toPlayer.y, toPlayer.x)))
        .SetTeam(HitboxTeam.ENEMY)
        .SetSize(new(1, 1))
        .SetDuration(0.5f)
        .SetVelocity(30 * toPlayer)
        .Build();
    }

    public void OnHit()
    {
        anim.SetBool("IsBeingAttacked", true);
        endStun = Time.time + 0.5f;
        rb.velocity = Vector2.zero;
    }

    public void EndAction()
    {
        actionable = true;
    }
}
