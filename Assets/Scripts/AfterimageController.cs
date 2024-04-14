using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterimageController : MonoBehaviour
{
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private SpriteRenderer targetSprite;
    private float imageTime;
    private float imageDelta;
    private float nextImage;
    private Color color;

    public void StartAfterImage(float duration, float delta, Color color) {
        imageTime = Time.time + duration;
        imageDelta = delta;
        nextImage = Time.time;
        this.color = color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time < imageTime) {
            if (Time.time > nextImage) {
                nextImage = Time.time + imageDelta;
                var afterImage = Instantiate(afterImagePrefab, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();

                afterImage.sprite = targetSprite.sprite;
                afterImage.color = color;
                afterImage.transform.GetComponent<DestroyAfterDelay>().Init(0.2f);
            }
        }
    }
}
