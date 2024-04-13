using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject hitboxPrefab;
    public HitboxData CreateHitbox(HitData data) {
        return new HitboxData().SetHitData(data);
    }

    public GameObject GetHitbox(Vector3 position, Quaternion rotation, Transform parent = null) {
        if (parent == null)
            return Instantiate(hitboxPrefab, position, rotation);
        else {
            var box = Instantiate(hitboxPrefab, parent);
            box.transform.localPosition = position;
            box.transform.localRotation = rotation;
            return box;
        }
    }
}
