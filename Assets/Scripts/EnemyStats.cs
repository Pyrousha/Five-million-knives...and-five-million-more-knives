using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float currHp;

    private void Start()
    {
        currHp = maxHp;
    }

    public void TakeDamage(HitData data)
    {
        currHp = Mathf.Max(0, currHp - data.damage);
        Debug.Log("Get they ass");

        if (currHp == 0)
        {
            EnemySpawner.Instance.OnEnemyKilled();
            Destroy(gameObject);
        }
    }
}
