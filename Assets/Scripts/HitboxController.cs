using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitboxController : MonoBehaviour
{
    [SerializeField] private UnityAction parentCallbacks;
    private Transform owner;
    private HitboxData data;
    public void Setup(Transform owner, HitboxData data) {
        this.owner = owner;
        this.data = data;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out HurtboxController hurtbox) && hurtbox.GetOwner() != owner) {
            hurtbox.FireOnHit(data);
            parentCallbacks.Invoke();
        }
    }
}
