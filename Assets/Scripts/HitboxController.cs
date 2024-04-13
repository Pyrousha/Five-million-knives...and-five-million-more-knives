using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitboxController : MonoBehaviour
{
    private HitData data;
    private HitboxTeam team;
    public void Setup(HitboxTeam team, HitData data) {
        this.team = team;
        this.data = data;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent(out HurtboxController hurtbox) && hurtbox.GetTeam() != team) {
            hurtbox.FireOnHit(data);
        }
    }
}

public enum HitboxTeam {
    NONE, PLAYER, ENEMY
}