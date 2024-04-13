using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private GroundedCheck ground;

    private bool grounded;
    void FixedUpdate()
    {
        grounded = ground.CheckGrounded();

        HandleInputs();
    }

    void HandleInputs() {
        var velocity = rbody.velocity;
        
        if (InputHandler.Instance.move.down) {
            velocity.x = InputHandler.Instance.dir * speed;
        } else {
            velocity.x = 0;
        }

        if (grounded && InputHandler.Instance.jump.pressed) {
            velocity.y = 10;
        }

        rbody.velocity = velocity;
    }
}
