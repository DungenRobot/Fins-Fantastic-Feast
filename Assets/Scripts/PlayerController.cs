using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditorInternal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    private float yVelocity = 0;

    public float moveSpeed = 500;

    [Range(0.0f, 50f)]
    public float jumpVelocity = 5;

    [Range(0.0f, 30f)]
    public float gravityJumpUp = 10;
    [Range(0.0f, 30f)]
    public float gravityJumpFall = 10;
    [Range(0.0f, 30f)]
    public float gravityFall = 10;

    private enum PlayerState {
        ON_FLOOR,
        JUMP_UP,
        JUMP_FALL,
        FALL
    };
    private PlayerState state;
    private float input;

    [Tooltip("How many seconds a jump input is stored for")]
    [Range(0.0f, 0.5f)]
    public double jumpCutoff = 0.2;
    private double jumpBuffer = 100; //used as the value to track jump input

    [Tooltip("How many seconds a player has to press jump when walking off platforms")]
    [Range(0.0f, 0.5f)]
    public double fallCutoff = 0.1;
    private double floorBuffer = 100;

    [Range(0.1f, 1.5f)]
    public double maxTimeInAir = 0.8;
    private double timeInAir = 0;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //get user input
        input = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Jump") > 0) { jumpBuffer = 0; }
    }

    void FixedUpdate()
    {

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
        //apply velocity based on input and stored velocities
        body.velocity = new Vector2(input * moveSpeed * Time.fixedDeltaTime, yVelocity);

        //update buffer values
        jumpBuffer += Time.fixedDeltaTime;
        floorBuffer += Time.fixedDeltaTime;
    }

    //user is on the floor
    private void StateOnFloor()
    {
        yVelocity = -gravityFall;

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
        yVelocity -= gravityJumpUp * Time.fixedDeltaTime;
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
        yVelocity -= gravityJumpFall * Time.fixedDeltaTime;

        bool falling = yVelocity < 0;

        if (falling)
        {
            state = PlayerState.FALL;
        }
    }

    private void StateFall()
    {
        yVelocity -= gravityFall * Time.fixedDeltaTime;

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
    }

    private bool IsOnGround()
    {
        const float playerWidth = 1.5f;
        const float playerHeight = 1;
        Vector2 playerPos = new(transform.position.x, transform.position.y);
        Vector2 boxSize = new(playerWidth - 0.2f, 0.3f);
        RaycastHit2D cast = Physics2D.BoxCast(playerPos, boxSize, 0f, Vector2.down, playerHeight / 1.9f);
        return cast.collider != null;
    }

}