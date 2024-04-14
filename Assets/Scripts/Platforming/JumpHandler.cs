using UnityEngine;

public class JumpHandler : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private float gravity;
    [Header("Jump Info")]
    [SerializeField] private float velocityScalar;
    [SerializeField] private float terminalVelocity;

    [SerializeField] private AnimationCurve risingCurve;
    [SerializeField] private AnimationCurve fallingCurve;
    
    //Extra variables
    private float timeStamp = -100;
    private bool jumping, starting;
    private AnimationCurve curve;
    private float risingTime, fallingTime, maxTime;

    private bool paused;
    private float pauseStarted;
    private float pausedUntil;
    private float storedVelocity;

    void Start()
    {
        rbody.gravityScale = gravity;

        risingTime = risingCurve[risingCurve.length - 1].time;
        fallingTime = fallingCurve[fallingCurve.length - 1].time;
    }

    void FixedUpdate()
    {
        if (Time.time < pausedUntil)
            return;

        if (paused) {
            paused = false;
            rbody.velocity = new Vector2(rbody.velocity.x, storedVelocity);
            timeStamp += pausedUntil - pauseStarted;
            rbody.gravityScale = gravity;
        }

        if (starting && (InputHandler.Instance.Jump.released || Time.time - timeStamp > risingTime))
            EndJump();
        if (Time.time - timeStamp <= maxTime)
            rbody.velocity = new Vector2(rbody.velocity.x, velocityScalar * curve.Evaluate(Time.time - timeStamp));

        if (rbody.velocity.y < -terminalVelocity)
            rbody.velocity = new Vector2(rbody.velocity.x, -terminalVelocity);
    }

    public void StartJump()
    {
        timeStamp = Time.time;
        starting = true;
        curve = risingCurve;
        maxTime = risingTime;
    }

    private void EndJump()
    {
        timeStamp = Time.time;
        starting = false;
        curve = fallingCurve;
        maxTime = fallingTime;
    }

    public void ForceLanding()
    {
        starting = false;
        timeStamp = -100;
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
            storedVelocity = rbody.velocity.y;
            rbody.velocity = new Vector2(rbody.velocity.x, 0);
            rbody.gravityScale = 0;
        }
    }

    public void DisableGravity() {
        rbody.gravityScale = 0;
    }

    public void ResetGravity() {
        rbody.gravityScale = gravity;
    }

}
