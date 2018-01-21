using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, HitboxHandler {
    public float Gravity = 35; // Downwards force. Higher = faster fall speed
    public float MaxFallSpeed = 25; // Can't fall faster than this
    public float JumpVelocity = 25; // Jump strength
    public float JumpResistance = 5; // How fast your jump ends (As drag)
    public float JumpManipulation = 5; // How much holding jump affects your jump
    public int MaxTotalJumps = 2; // How many jumps you get after touching a surface
    public float WallJumpXForce = 10; // When wall jumping, how hard you push off
    public float GroundMovementSpeed = 7; // Vertical move speed on the ground
    public float AirControlForce = 65; // Amount of control you have mid air
    public float MaxAirControlSpeed = 10; // How fast can you move in the air using controls?
    public float WallSlideSpeed = 7; // How fast you slide on the wall
    public float WallSlideDrag = 1; // How fast you slow down when grabbing the wall

    // Internals
    private enum MoveState { LEFT, RIGHT, NEUTRAL, RIGHT_WALL_SLIDE, LEFT_WALL_SLIDE };
    private enum WallType { LEFT, RIGHT, NONE };
    private Rigidbody2D rb;

    // Physics switches
    private MoveState curMoveState = MoveState.NEUTRAL; // Current movement state
    private WallType currentWallCollision = WallType.NONE; // Current type of wall collision
    private int curJumpCount = 2; // Number of jumps currently available to player
    private bool grounded = false; // Wheter or not player is grounded
    private int wallCollisionCount; // Used to only set wall to NONE if no walls are being collided with
    private bool jumpKeyHeld = false;

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
            wallCollisionCount++;
        }
        if (hitboxCollider.gameObject.name == "LeftWallHitbox" && foreignCollider.CompareTag("Wall"))
        {
            currentWallCollision = WallType.LEFT;
            wallCollisionCount++;
        }
    }

    public void OnHitboxExit(Collider2D hitboxCollider, Collider2D foreignCollider)
    {
        // Assumes we only collide with one wall type at a time
        if ((hitboxCollider.gameObject.name == "RightWallHitbox" || hitboxCollider.gameObject.name == "LeftWallHitbox") && foreignCollider.CompareTag("Wall"))
        {
            wallCollisionCount--;
            if (wallCollisionCount <= 0) currentWallCollision = WallType.NONE; // Only set to none if it's not colliding with any walls
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Floor"))
        {
            curJumpCount = MaxTotalJumps;
        }
        if (collision.collider.CompareTag("Floor"))
        {
            grounded = true;
        }
        if (collision.collider.CompareTag("Deadly"))
        {
            Kill();
            GameSystem.instance.GameOver();
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

    public void Kill()
    {
        gameObject.SetActive(false);
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
            jumpKeyHeld = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpKeyHeld = false;
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

        // When moving upwards
        if (rb.velocity.y > 0)
        {
            // Apply resistance
            Vector2 dragForce = new Vector2(0, 0);
            dragForce.y = -JumpResistance * rb.velocity.y;
            rb.AddForce(dragForce);

            // Apply jump manipulation
            if (jumpKeyHeld)
            {
                rb.AddForce(new Vector2(0, JumpManipulation));
            }
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
        else if (rb.velocity.y > -WallSlideSpeed - Time.fixedDeltaTime * Gravity)
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
