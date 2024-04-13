using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private UnityEvent<HitData> onHit;
    [SerializeField] private HitboxTeam team;
    public void FireOnHit(HitData data) {
        onHit.Invoke(data);
    }

    public HitboxTeam GetTeam() {
        return team;
    }
}
