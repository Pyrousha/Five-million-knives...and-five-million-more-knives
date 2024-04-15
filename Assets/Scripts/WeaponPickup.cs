using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private WeaponType type;
    void Awake()
    {
        type = (WeaponType)Random.Range(1, (int)WeaponType.MAX);
        animator.SetTrigger(type.ToString());
    }

    public void OnPickup() {
        GameManager.Instance.GetPlayer().PickupWeapon(type);
        Destroy(gameObject);
    }
}
