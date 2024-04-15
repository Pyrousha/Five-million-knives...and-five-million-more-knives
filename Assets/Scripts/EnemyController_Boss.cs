using UnityEngine;

public class EnemyController_Boss : MonoBehaviour
{
    private Rigidbody2D rb;
    private EnemyStats stats;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprRend;

    [Space(10)]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float attackWaitTime;

    private Transform player;

    private bool actionable;

    private float toPlayerDirX;

    private float nextAttackTime;

    private Transform lSpawnPos;
    private Transform rSpawnPos;

    private bool onLeftSide;

    private AttackIndices currAttackIndex;

    private enum AttackIndices
    {
        Slide = 0,
        Plunge = 1,
        Burst = 2
    }


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
        nextAttackTime = Time.time + attackWaitTime;

        EnemySpawner.Instance.OnEnemySpawned();
    }

    private void FixedUpdate()
    {
        if (actionable && !stats.IsDead && Time.time >= nextAttackTime)
        {
            //Do Attack

            bool currentlyOnLeft = onLeftSide;

            onLeftSide = (Random.Range(0, 2) == 0);
            if (onLeftSide != currentlyOnLeft || currAttackIndex == AttackIndices.Plunge) //Should always teleport after plunge
            {
                //Do teleport
                anim.SetBool("Teleporting", true);
            }
            else
            {
                //No teleport needed
                anim.SetBool("Teleporting", false);
            }

            sprRend.flipX = !onLeftSide;

            currAttackIndex = (AttackIndices)Random.Range(0, 3);
            switch (currAttackIndex)
            {
                case AttackIndices.Slide:
                    //Slide
                    anim.SetTrigger("Attack_Slide");
                    break;

                case AttackIndices.Plunge:

                    //Plunge
                    anim.SetTrigger("Attack_Plunge");
                    break;

                case AttackIndices.Burst:
                    //Burst
                    anim.SetTrigger("Attack_Burst");
                    break;
            }

        }
    }

    public void OnTeleportEnd()
    {
        switch (currAttackIndex)
        {
            case AttackIndices.Slide:
                break;

            case AttackIndices.Plunge:
                break;

            case AttackIndices.Burst:
                break;
        }
    }

    public void FireSlideAttack()
    {

    }

    public void FirePlungeAttack()
    {

    }

    public void FireBurstAttack()
    {

    }

    public void FireAttack()
    {
        //GameManager.Instance.CreateHitbox(new HitData() { damage = 1 })
        //.SetParent(transform)
        //.SetTeam(HitboxTeam.ENEMY)
        //.SetSize(new Vector2(2, 3))
        //.SetPos(new Vector3(toPlayerDirX, 1.5f, 0))
        //.SetDuration(0.25f)
        //.Build();

        //rb.velocity = new Vector2(attackScootSpeedX * toPlayerDirX, attackScootSpeedY);
    }

    public void EndAction()
    {
        actionable = true;

        nextAttackTime = Time.time + attackWaitTime;
    }
}
