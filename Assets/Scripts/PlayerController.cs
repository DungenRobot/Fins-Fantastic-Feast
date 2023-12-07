using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AudioSource jumpSoundEffect;
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprite;
    private float yVelocity = 0;
    private float yAcceleration = 0;
    private float damage_effector = 0;

    private enum PlayerState {
        ON_FLOOR,
        JUMP_UP,
        JUMP_FALL,
        FALL
    };
    private PlayerState state;
    private float input;

    [Header("The Basics")]
    [Tooltip("How quickly the player moves")]
    public float moveSpeed = 500;

    [Tooltip("How much force the player jumps with")]
    [Range(0.0f, 50f)]
    public float jumpVelocity = 5;

    [Tooltip("How much force the player jumps with")]
    [Range(0.0f, 50f)]
    public float knockback_amount = 500;
    


    [Header("Gravity")]
    [Tooltip("Gravity while player is moving up and holding up")]
    [Range(0.0f, 80f)]
    public float gravityJumpUp = 10;
    [Tooltip("Gravity when jump is released and player is moving up")]
    [Range(0.0f, 80f)]
    public float gravityJumpFall = 10;
    [Tooltip("Gravity when player is falling")]
    [Range(0.0f, 80f)]
    public float gravityFall = 10;

    [Header("Timing Windows")]
    [Tooltip("How many seconds a jump input is stored for")]
    [Range(0.0f, 0.2f)]
    public double jumpCutoff = 0.2;
    private double jumpBuffer = 100; //used as the value to track jump input

    [Tooltip("Number of extra seconds a player can jump after falling")]
    [Range(0.0f, 0.2f)]
    public double fallCutoff = 0.1;
    private double floorBuffer = 100;

    [Tooltip("Maximum time a player can hold jump before airtime ends")]
    [Range(0.1f, 1.0f)]
    public double maxTimeInAir = 0.8;
    private double timeInAir = 0;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //get user input
        input = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Jump") > 0) { jumpBuffer = 0; }
        animator.SetBool("is_moving", (input != 0));
        animator.SetInteger("State", (int) state);

        if (input > 0)
        {
            sprite.flipX = false;
        }
        else if (input < 0)
        {
            sprite.flipX = true;
        }
    }

    void FixedUpdate()
    {
        yAcceleration = 0;

        if (IsOnGround()) { floorBuffer = 0; }

        //switch into appropriate state logic
        switch (state)
        {
            case PlayerState.ON_FLOOR:
                StateOnFloor();
                break;
            case PlayerState.JUMP_UP:
                StateJumpUp();
                break;
            case PlayerState.JUMP_FALL:
                StateJumpFall();
                break;
            case PlayerState.FALL:
                StateFall();
                break;
        }

        //acceleration applied in two steps for discrete physics reasons
        //A framerate that changes often would get timing wrong without this 
        // (Time.deltaTime != this frame, Time.deltaTime = last frame)
        // This technically isn't correct either because we're modifying the acceleration but I have limits

        
        yVelocity -= yAcceleration / 2;

        //apply velocity based on input and stored velocities
        body.velocity = new Vector2(input * moveSpeed * Time.fixedDeltaTime, yVelocity);

        if (Math.Abs(damage_effector) > 0.1) body.velocity = new Vector2(damage_effector, yVelocity);
        damage_effector *= 0.9f;

        yVelocity -= yAcceleration / 2;

        //update buffer values
        jumpBuffer += Time.fixedDeltaTime;
        floorBuffer += Time.fixedDeltaTime;
    }

    //user is on the floor
    private void StateOnFloor()
    {
        yVelocity = -gravityFall / 100;

        bool userJumps = jumpBuffer <= jumpCutoff;
        bool notOnFloor = floorBuffer != 0;

        if (userJumps)
        {
            DoJump();
        } 
        else if ( notOnFloor )
        {
            state = PlayerState.FALL;
        }
    }

    //user is jumping while holding the jump button
    //a lower gravity is applied here
    private void StateJumpUp()
    {
        yAcceleration = gravityJumpUp * Time.fixedDeltaTime;
        timeInAir += Time.fixedDeltaTime;

        if (timeInAir > maxTimeInAir)
        {
            state = PlayerState.JUMP_FALL;
            return;
        }

        bool falling = yVelocity < 0;
        bool userReleasedJump = jumpBuffer != 0;

        if (falling)
        {
            state = PlayerState.FALL;
        }
        //user releases jump button
        else if (userReleasedJump)
        {
            state = PlayerState.JUMP_FALL;
        }

    }

    private void StateJumpFall()
    {
        yAcceleration = gravityJumpFall * Time.fixedDeltaTime;

        bool falling = yVelocity < 0;

        if (falling)
        {
            state = PlayerState.FALL;
        }
    }

    private void StateFall()
    {
        yAcceleration = gravityFall * Time.fixedDeltaTime;

        bool stillCanJump = floorBuffer < fallCutoff;
        bool wantsToJump = jumpBuffer < jumpCutoff;

        if (stillCanJump && wantsToJump)
        {
            DoJump();
        }
        else if (floorBuffer == 0)
        {
            state = PlayerState.ON_FLOOR;
        }
    }

    private void DoJump()
    {
        yVelocity = jumpVelocity;
        timeInAir = 0;
        //jump buffer set to an arbitrary value greater than jump cutoff
        //this prevents cases where a second jump results from a single input
        jumpBuffer = jumpCutoff * 2; 
        state = PlayerState.JUMP_UP;
        //Bridge puts Audio
        jumpSoundEffect.Play();
    }

    private bool IsOnGround()
    {
        const float playerWidth = 1.2f;
        const float playerHeight = 0.7f;
        Vector2 playerPos = new(transform.position.x, transform.position.y);
        Vector2 boxSize = new(playerWidth - 0.2f, 0.3f);
        RaycastHit2D cast = Physics2D.BoxCast(playerPos, boxSize, 0f, Vector2.down, playerHeight / 1.9f);
        
        if (cast.collider == null) {
            return false;
        }

        return cast.collider.gameObject.CompareTag("floor");
    }

    void takeDamage()
    {
        if (GetComponent<PlayerHealth>().isInv) return;
        damage_effector = knockback_amount;
        if (!sprite.flipX) damage_effector *= -1;
        yVelocity = knockback_amount / 2f;

    }

}