using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float currHp;

    public bool IsDead { get; private set; } = false;

    private void Start()
    {
        currHp = maxHp;
    }

    public void TakeDamage(HitData data)
    {
        if (IsDead)
            return;

        currHp = Mathf.Max(0, currHp - data.damage);
        //Debug.Log("Get they ass");

        if (currHp == 0)
        {
            IsDead = true;
            EnemySpawner.Instance.OnEnemyKilled();
            Destroy(gameObject);
        }
    }
}
