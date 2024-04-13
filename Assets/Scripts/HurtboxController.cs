using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HurtboxController : MonoBehaviour
{
    [SerializeField] private UnityAction<HitboxData> onHit;
    public void FireOnHit(HitboxData data) {
        onHit.Invoke(data);
    }

    public Transform GetOwner() {
        return transform.parent;
    }

}
