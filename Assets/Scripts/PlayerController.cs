using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, HitboxHandler {
    public float Gravity = 100;
    public float MaxFallSpeed = 1000;
    public float JumpVelocity = 25;
    public int MaxJumps = 2;
    public float WallJumpXForce = 15;
    public float GroundMovementSpeed = 7;
    public float AirControlForce = 50;
    public float MaxAirControlSpeed = 10;
    public float WallSlideSpeed = 2;
    public float WallSlideDrag = 5;

    // Internals
    private enum MoveState { LEFT, RIGHT, NEUTRAL, RIGHT_WALL_SLIDE, LEFT_WALL_SLIDE };
    private enum WallType { LEFT, RIGHT, NONE };
    private Rigidbody2D rb;

    // Physics switches
    private MoveState curMoveState = MoveState.NEUTRAL;
    private WallType currentWallCollision = WallType.NONE;
    private int curJumpCount = 0;
    private bool grounded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Behavior
    void Update () {
        handleInput();
	}

    // Physics
    void FixedUpdate()
    {
        applyGravity();
        applyMovement();
    }

    public void OnHitboxEnter(Collider2D hitboxCollider, Collider2D foreignCollider)
    {
        // Assumes we only collide with one wall type at a time
        if (hitboxCollider.gameObject.name == "RightWallHitbox" && foreignCollider.CompareTag("Wall"))
        {
            currentWallCollision = WallType.RIGHT;
        }
        if (hitboxCollider.gameObject.name == "LeftWallHitbox" && foreignCollider.CompareTag("Wall"))
        {
            currentWallCollision = WallType.LEFT;
        }
    }

    public void OnHitboxExit(Collider2D hitboxCollider, Collider2D foreignCollider)
    {
        // Assumes we only collide with one wall type at a time
        if ((hitboxCollider.gameObject.name == "RightWallHitbox" || hitboxCollider.gameObject.name == "LeftWallHitbox") && foreignCollider.CompareTag("Wall"))
        {
            currentWallCollision = WallType.NONE;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Floor"))
        {
            curJumpCount = MaxJumps;
        }
        if (collision.collider.CompareTag("Floor"))
        {
            grounded = true;
        }
    }

    public void Jump()
    {
        if (curJumpCount > 0)
        {
            float xVel = rb.velocity.x;
            if (curMoveState == MoveState.LEFT_WALL_SLIDE)
            {
                xVel = WallJumpXForce;
                curMoveState = MoveState.RIGHT;
            }
            if (curMoveState == MoveState.RIGHT_WALL_SLIDE)
            {
                xVel = -WallJumpXForce;
                curMoveState = MoveState.LEFT;
            }
            rb.velocity = new Vector2(xVel, JumpVelocity);
            grounded = false;
            curJumpCount--;
        }
    }
    
    private void handleInput()
    {
        // Movement - can't run inside update() because we are directly updating position
        // So instead we toggle a variable and handle it in fixed update
        if (Input.GetKeyDown(KeyCode.D))
        {
            curMoveState = MoveState.RIGHT;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            curMoveState = MoveState.LEFT;
        }
        // If we release a key check if the other key is still pressed and go back to that direction
        // otherwise stop
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            // These ifs can be exclusive because we know at least one key has been released
            if (Input.GetKey(KeyCode.A))
            {
                curMoveState = MoveState.LEFT;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                curMoveState = MoveState.RIGHT;
            }
            else
            {
                curMoveState = MoveState.NEUTRAL;
            }
        }

        // Check if we're wall sliding
        // Wall sliding occurs if we're moving into the wall and holding the movement key into the wall
        // This overrides previous movement state
        if (currentWallCollision == WallType.RIGHT && curMoveState == MoveState.RIGHT)
        {
            curMoveState = MoveState.RIGHT_WALL_SLIDE;
        }
        else if (currentWallCollision == WallType.LEFT && curMoveState == MoveState.LEFT)
        {
            curMoveState = MoveState.LEFT_WALL_SLIDE;
        }
        if (currentWallCollision == WallType.NONE)
        {
            // These ifs can be exclusive because we know at least one key has been released
            if (Input.GetKey(KeyCode.A))
            {
                curMoveState = MoveState.LEFT;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                curMoveState = MoveState.RIGHT;
            }
            else
            {
                curMoveState = MoveState.NEUTRAL;
            }
        }

        // Jump - this can run inside update() because it's modifying a variable
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void applyMovement()
    {
        // Movement state handling
        switch(curMoveState)
        {
            // Moving Right
            case MoveState.RIGHT:
                moveRight();
                break;
            // Moving Left
            case MoveState.LEFT:
                moveLeft();
                break;
            case MoveState.LEFT_WALL_SLIDE:
                moveLeft();
                wallSlideLeft();
                break;
            case MoveState.RIGHT_WALL_SLIDE:
                moveRight();
                wallSlideRight();
                break;
            case MoveState.NEUTRAL:
                if (grounded) rb.velocity = new Vector2(0, rb.velocity.y);
                break;
        }

        // If above max fall speed, clamp
        if (rb.velocity.y < -MaxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -MaxFallSpeed);
        }
    }

    private void applyGravity()
    {
        rb.velocity = rb.velocity + Time.fixedDeltaTime * new Vector2(0, -Gravity);
    }

    private void moveRight()
    {
        if (grounded)
        {
            rb.velocity = new Vector2(GroundMovementSpeed, rb.velocity.y);
        }
        else
        {
            Vector2 newVel = rb.velocity + new Vector2(Time.fixedDeltaTime * AirControlForce, 0);
            if (newVel.x <= MaxAirControlSpeed)
            {
                rb.velocity = newVel;
            }
        }
    }

    private void moveLeft()
    {
        if (grounded)
        {
            rb.velocity = new Vector2(-GroundMovementSpeed, rb.velocity.y);
        }
        else
        {
            Vector2 newVel = rb.velocity + new Vector2(Time.fixedDeltaTime * -AirControlForce, 0);
            if (newVel.x >= -MaxAirControlSpeed)
            {
                rb.velocity = newVel;
            }
        }
    }

    private void wallSlideLeft()
    {
        wallSlidePhysics();
    }

    private void wallSlideRight()
    {
        wallSlidePhysics();
    }

    private void wallSlidePhysics()
    {
        // If we're falling slower than wallslide speed after gravity is applied, continue as normal
        if (rb.velocity.y > -WallSlideSpeed) return;
        // else if gravity only takes us a little past target speed just clip to that speed
        else if (rb.velocity.y > -WallSlideSpeed - Gravity)
        {
            rb.velocity = new Vector2(rb.velocity.x, -WallSlideSpeed);
        }
        // Otherwise cancel the effects of gravity and apply wall friction
        else
        {
            rb.velocity = rb.velocity - Time.fixedDeltaTime * new Vector2(0, -Gravity); // cancel gravity

            // Apply wall friction
            Vector2 wallDragForce = new Vector2(0, 0);
            wallDragForce.y = WallSlideDrag;
            rb.AddForce(wallDragForce);
        }
    }
}
