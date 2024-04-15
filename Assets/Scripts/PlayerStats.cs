using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider summonSlider;
    [SerializeField] private Image summonFill;

    [SerializeField] private float maxHp;
    private float currHp;

    [SerializeField] private float maxSp;
    private float currSp;

    public static float SUMMON_COST = 1;
    private bool dead = false;

    [SerializeField] private Color summon_active;
    [SerializeField] private Color summon_inactive;

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

    public void GainSP(float _spToGain)
    {
        currSp = Mathf.Min(maxSp, currSp + _spToGain);
        UpdateSPSlider();
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
        if (currSp < SUMMON_COST)
            summonFill.color = summon_inactive;
        else
            summonFill.color = summon_active;

        summonSlider.value = currSp / maxSp;
    }
}
