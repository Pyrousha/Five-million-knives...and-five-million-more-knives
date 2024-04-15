using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitboxController : MonoBehaviour
{
    private HitData data;
    private HitboxTeam team;
    bool ready = false;
    public void Setup(HitboxTeam team, HitData data) {
        this.team = team;
        this.data = data;
        ready = true;
    }

    void OnTriggerEnter2D(Collider2D other) {
        StartCoroutine(OnHit(other));
    }

    private IEnumerator OnHit(Collider2D other) {
        yield return new WaitUntil(() => ready);

        if (other.TryGetComponent(out HurtboxController hurtbox) && hurtbox.GetTeam() != team) {
            hurtbox.FireOnHit(data);
        }
    }
}

public enum HitboxTeam {
    NONE, PLAYER, ENEMY
}