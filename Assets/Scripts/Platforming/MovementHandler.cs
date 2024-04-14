using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private AnimationCurve accelerationCurve;
    private float accelerationTime;
    [SerializeField] private AnimationCurve decelerationCurve;
    private float decelerationTime;
    private float timestamp;
    private float dir;
    private float decelSpeed;
    bool moving = false;
    bool paused;
    float pauseStarted;
    float pausedUntil;
    float storedVelocity;
    float curSpeed;


    void Awake() {
        accelerationTime = accelerationCurve[accelerationCurve.length - 1].time;
        decelerationTime = decelerationCurve[decelerationCurve.length - 1].time;
    }

    void FixedUpdate()
    {
        if (Time.time < pausedUntil)
            return;

        if (paused) {
            paused = false;
            rbody.velocity = new Vector2(storedVelocity, rbody.velocity.y);
            timestamp += pausedUntil - pauseStarted;
        }

        if (Time.time < timestamp) {
            if (moving)
                rbody.velocity = new Vector2(speed * dir * accelerationCurve.Evaluate(Time.time - timestamp + accelerationTime), rbody.velocity.y);
            else
                rbody.velocity = new Vector2(decelSpeed * dir * decelerationCurve.Evaluate(Time.time - timestamp + decelerationTime), rbody.velocity.y);
        } else {
            if (moving)
                rbody.velocity = new Vector2(curSpeed * dir, rbody.velocity.y);
        }
    }

    public void StartDeceleration() {
        moving = false;
        timestamp = Time.time + decelerationTime;
        decelSpeed = Mathf.Abs(rbody.velocity.x);
        if (Mathf.Abs(rbody.velocity.x) < decelSpeed)
            decelSpeed = Mathf.Abs(rbody.velocity.x);
    }

    public void StartAcceleration(float dir) {
        moving = true;
        timestamp = Time.time + accelerationTime;
    }

    public void UpdateMovement(float dir) {
        curSpeed = Mathf.Abs(rbody.velocity.x);
        if (curSpeed > speed) {
            curSpeed -= (curSpeed - speed) * 0.1f;
        } else 
            curSpeed = speed;

        if (this.dir != dir)
            curSpeed = speed;
        this.dir = dir;
        moving = true;
    }

    public void Pause(float duration) {
        float endPause = Time.time + duration;

        if (paused) {
            if (endPause > pausedUntil)
                pausedUntil = endPause;
        } else {
            paused = true;
            pauseStarted = Time.time;
            pausedUntil = endPause;
            storedVelocity = rbody.velocity.x;
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }
    }

    public void ForceStop() {
        moving = false;
        timestamp = 0;
        rbody.velocity = rbody.velocity.y * Vector2.up;
    }

    public void ForceDir(float dir) {
        this.dir = dir;
    }

}
