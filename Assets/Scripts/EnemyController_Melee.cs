using UnityEngine;

public class EnemyController_Melee : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyStats stats;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprRend;

    [Space(10)]
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float frictionSpeed;
    [SerializeField] private float gravSpeed;
    [SerializeField] private float attackScootSpeedX;
    [SerializeField] private float attackScootSpeedY;
    [SerializeField] private float playerRangeX;
    [SerializeField] private float playerRangeY;

    private Transform player;

    private bool actionable;
    private float endStun;

    private float toPlayerDirX;

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
    }

    private void FixedUpdate()
    {
        float currSpeed = rb.velocity.x;
        float currSpeedAbs = Mathf.Abs(currSpeed);

        if (currSpeed != 0)
            currSpeed = (currSpeed / currSpeedAbs) * Mathf.Max(0, currSpeedAbs - frictionSpeed);

        if (currSpeed > 0)
            sprRend.flipX = false;
        else if (currSpeed < 0)
            sprRend.flipX = true;

        if (Time.time >= endStun)
        {
            anim.SetBool("IsBeingAttacked", false);

            if (actionable && !stats.IsDead && Time.time >= endStun)
            {
                float toPlayer = (player.position - transform.position).x;

                if (toPlayer < -playerRangeX)
                {
                    //Try move left
                    anim.SetBool("Moving", true);

                    if (currSpeed > -maxMoveSpeed)
                    {
                        //Accelerate towards max
                        currSpeed = Mathf.Max(-maxMoveSpeed, currSpeed - accelSpeed);
                    }
                }
                else if (toPlayer > playerRangeX)
                {
                    //Try move right
                    anim.SetBool("Moving", true);

                    if (currSpeed < maxMoveSpeed)
                    {
                        //Accelerate towards max
                        currSpeed = Mathf.Min(maxMoveSpeed, currSpeed + accelSpeed);
                    }
                }
                else
                {
                    //Try attack player

                    if (Mathf.Abs(transform.position.y - player.position.y) < playerRangeY)
                    {
                        anim.SetBool("Moving", false);
                        anim.SetTrigger("Attack");

                        toPlayerDirX = toPlayer / Mathf.Abs(toPlayer);

                        actionable = false;
                    }
                }
            }
        }

        rb.velocity = new Vector2(currSpeed, rb.velocity.y - gravSpeed);
    }

    public void FireAttack()
    {
        GameManager.Instance.CreateHitbox(new HitData() { damage = 1 })
        .SetParent(transform)
        .SetTeam(HitboxTeam.ENEMY)
        .SetSize(new Vector2(2, 3))
        .SetPos(new Vector3(toPlayerDirX, 1.5f, 0))
        .SetDuration(0.25f)
        .Build();

        rb.velocity = new Vector2(attackScootSpeedX * toPlayerDirX, attackScootSpeedY);
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
