using UnityEngine;

public struct HitboxData
{
    public static float MATCH_ANIM_DURATION = -1;
    private HitboxAnim anim;
    private HitboxTeam team;
    private Transform parent;
    private Vector3 pos;
    private Quaternion rotation;
    private HitData hitData;
    private Vector2 hitboxSize;
    private float duration;

    public HitboxData SetHitData(HitData data)
    {
        this.hitData = data;
        return this;
    }

    public HitboxData SetTeam(HitboxTeam team)
    {
        this.team = team;
        return this;
    }

    public HitboxData SetPos(Vector3 pos)
    {
        this.pos = pos;
        return this;
    }

    public HitboxData SetParent(Transform parent)
    {
        this.parent = parent;
        return this;
    }

    public HitboxData SetRotation(Vector3 eulers)
    {
        rotation = Quaternion.Euler(eulers);
        return this;
    }

    public HitboxData SetSize(Vector2 size)
    {
        hitboxSize = size;
        return this;
    }

    public HitboxData SetDuration(float duration)
    {
        this.duration = duration;
        return this;
    }

    public HitboxData SetAnimation(HitboxAnim anim) {
        this.anim = anim;
        return this;
    }

    public void Build()
    {
        var box = GameManager.Instance.GetHitbox(pos, rotation, parent, anim);
        box.GetComponent<HitboxController>().Setup(team, hitData);

        if (hitboxSize != default(Vector2))
            box.GetComponent<BoxCollider2D>().size = hitboxSize;

        if (duration != 0)
        {
            if (duration == MATCH_ANIM_DURATION)
                duration = box.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length - 0.05f;
            box.AddComponent<DestroyAfterDelay>().Init(duration);
        }
    }
}

public struct HitData
{
    public int damage;

    public HitData(int damage)
    {
        this.damage = damage;
    }
}
