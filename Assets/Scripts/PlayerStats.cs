using BeauRoutine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider summonSlider;

    [SerializeField] private float maxHp;
    private float currHp;

    [SerializeField] private float maxSp;
    private float currSp;

    public static float SUMMON_COST = 1;
    private bool dead = false;


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currHp = maxHp;
        currSp = maxSp;

        UpdateHPSlider();
        UpdateSPSlider();
    }

    public void TakeDamage(float _dmg)
    {
        if (dead)
            return;

        currHp = Mathf.Max(0, currHp - _dmg);
        UpdateHPSlider();

        if (currHp == 0)
        {
            dead = true;
            GameplayPopupController.Instance.OnPlayerDeath();
        }
    }

    public bool TryConsumeStamina(float amt)
    {
        if (currSp < amt)
            return false;

        currSp -= amt;
        UpdateSPSlider();

        return true;
    }

    private IEnumerator SummonRoutine()
    {
        yield return null;

        //TODO: spawn stuff
        playerController.EndAction();
    }

    private void UpdateHPSlider()
    {
        hpSlider.value = currHp / maxHp;
    }

    private void UpdateSPSlider()
    {
        summonSlider.value = currSp / maxSp;
    }
}
