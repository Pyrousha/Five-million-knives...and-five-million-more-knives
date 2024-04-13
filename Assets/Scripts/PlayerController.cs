using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private GroundedCheck ground;
    [SerializeField] private MovementHandler movement;
    [SerializeField] private JumpHandler jump;

    private bool grounded;

    void Start() {
        GameManager.Instance.CreateHitbox(new HitData())
        .SetTeam(HitboxTeam.PLAYER)
        .Build();
    }

    void FixedUpdate()
    {
        grounded = ground.CheckGrounded();

        HandleInputs();
    }

    void HandleInputs() {
        var velocity = rbody.velocity;
        
        if (InputHandler.Instance.move.pressed) {
            movement.StartAcceleration(InputHandler.Instance.dir);
        } else if (InputHandler.Instance.move.down) {
            movement.UpdateMovement(InputHandler.Instance.dir);
        } else {
            movement.StartDeceleration();
        }

        if (grounded && InputHandler.Instance.jump.pressed) {
            jump.StartJump();
        }

        rbody.velocity = velocity;
    }

    public void OnHit(HitData data) {
        Debug.Log("Yeouch!");
    }
}
