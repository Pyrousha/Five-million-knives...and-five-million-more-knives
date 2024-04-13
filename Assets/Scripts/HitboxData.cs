using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxData
{
    private HitboxTeam team;
    private Transform parent;
    private Vector3 pos;
    private Quaternion rotation;
    private HitData hitData;
    private Vector2 hitboxSize;

    public HitboxData SetHitData(HitData data) {
        this.hitData = data;
        return this;
    }

    public HitboxData SetTeam(HitboxTeam team) {
        this.team = team;
        return this;
    }

    public HitboxData SetPos(Vector3 pos) {
        this.pos = pos;
        return this;
    }

    public HitboxData SetParent(Transform parent) {
        this.parent = parent;
        return this;
    }

    public HitboxData SetRotation(Vector3 eulers) {
        rotation = Quaternion.Euler(eulers);
        return this;
    }

    public HitboxData SetSize(Vector2 size) {
        hitboxSize = size;
        return this;
    }

    public void Build() {
        var box = GameManager.Instance.GetHitbox(pos, rotation, parent);
        box.GetComponent<HitboxController>().Setup(team, hitData);
        if (hitboxSize != default(Vector2))
            box.GetComponent<BoxCollider2D>().size = hitboxSize;
    }
}

public struct HitData {
    public int damage;
}
