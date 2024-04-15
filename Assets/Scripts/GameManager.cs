using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private List<AnimationClip> hitboxAnims;

    private PlayerController player;
    public HitboxData CreateHitbox(HitData data)
    {
        return new HitboxData().SetHitData(data);
    }

    public PlayerController GetPlayer()
    {
        return player;
    }

    public void RegisterPlayer(PlayerController player)
    {
        this.player = player;
    }

    public GameObject GetHitbox(Vector3 position, Quaternion rotation, Transform parent = null, HitboxAnim animationID = HitboxAnim.DEFAULT)
    {
        GameObject box = null;
        if (parent == null)
        {
            box = Instantiate(hitboxPrefab, position, rotation);
        }
        else
        {
            box = Instantiate(hitboxPrefab, parent);
            box.transform.localPosition = position;
            box.transform.localRotation = rotation;
        }

        if (animationID != HitboxAnim.DEFAULT)
        {
            Animator animator = box.GetComponent<Animator>();
            AnimatorOverrideController animController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animController["anim"] = hitboxAnims[(int)animationID - 1];
            animator.runtimeAnimatorController = animController;
        }

        return box;
    }
}

public enum HitboxAnim
{
    DEFAULT, PLAYER_SWORD, PLAYER_ARROW, ENEMY_PROJECTILE
}
