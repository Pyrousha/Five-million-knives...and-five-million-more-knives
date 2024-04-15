using BeauRoutine;
using System.Collections;
using UnityEngine;

public class EnemyController_Melee : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyStats stats;

    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float frictionSpeed;
    [SerializeField] private float gravSpeed;

    [SerializeField] private SpriteRenderer sprRend;

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
        player = PlayerController.Instance.transform;

        actionable = true;

        EnemySpawner.Instance.OnEnemySpawned();
    }

    private void FixedUpdate()
    {
        if (Time.time < endStun)
            return;

        float toPlayer = (player.position - transform.position).x;

        float currSpeed = rb.velocity.x;
        float currSpeedAbs = Mathf.Abs(currSpeed);

        if (currSpeed != 0)
            currSpeed = (currSpeed / currSpeedAbs) * Mathf.Max(0, currSpeedAbs - frictionSpeed);

        if (currSpeed > 0)
            sprRend.flipX = false;
        else if (currSpeed < 0)
            sprRend.flipX = true;

        if (actionable && !stats.IsDead)
        {
            if (toPlayer < -1f)
            {
                //Try move left

                if (currSpeed > -maxMoveSpeed)
                {
                    //Accelerate towards max
                    currSpeed = Mathf.Max(-maxMoveSpeed, currSpeed - accelSpeed);
                }
            }
            else if (toPlayer > 1f)
            {
                //Try move right

                if (currSpeed < maxMoveSpeed)
                {
                    //Accelerate towards max
                    currSpeed = Mathf.Min(maxMoveSpeed, currSpeed + accelSpeed);
                }
            }
            else
            {
                //Try attack player

                if ((transform.position - player.position).magnitude < 3f)
                {
                    Routine.Start(this, AttackRoutine(new Vector3(toPlayer / Mathf.Abs(toPlayer) * 0.95f, 0, 0)));
                    actionable = false;
                }
            }
        }

        rb.velocity = new Vector2(currSpeed, rb.velocity.y - gravSpeed);
    }

    private IEnumerator AttackRoutine(Vector3 pos)
    {
        GameManager.Instance.CreateHitbox(new HitData(1))
            .SetParent(transform)
            .SetTeam(HitboxTeam.ENEMY)
            .SetSize(new Vector2(2, 3))
            .SetPos(pos + new Vector3(0, 1.5f, 0))
            .SetDuration(0.5f)
            .Build();

        yield return 1;
        actionable = true;
    }

    public void OnHit()
    {
        endStun = Time.time + 0.5f;
        rb.velocity = Vector2.zero;
    }
}
