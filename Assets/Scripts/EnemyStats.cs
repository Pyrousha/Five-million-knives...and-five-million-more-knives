using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float currHp;

    public void TakeDamage(float _dmg)
    {
        currHp = Mathf.Max(0, currHp - _dmg);

        if (currHp == 0)
        {
            Destroy(gameObject);
        }
    }
}
