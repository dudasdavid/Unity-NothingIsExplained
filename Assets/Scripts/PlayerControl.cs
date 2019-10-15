﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float gravityFall;
    public float velocity;
    public float velocityIncrement;
    public float jumpVelocity;
    public float doubleJumpVelocity;
    public float slowDown;
    public Rigidbody2D rigidBody;

    private Vector2 velocityVector2D = Vector2.zero;
    private int jumpFlag = 0;
    private int forwardFlag = 0;
    private int backwardFlag = 0;
    private int duckFlag = 0;

    private int jumpCount;
    private bool isJumping;
    private bool isCrouching;

    public bool isDoubleJumpEnabled;
    public bool invertGravity;

    private Animator m_Animator;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow ) || Input.GetKeyDown(KeyCode.Space))
        {
            jumpFlag = 1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            forwardFlag = 1;
            backwardFlag = 0;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            forwardFlag = 0;
            backwardFlag = 1;
        }
        else
        {
            forwardFlag = 0;
            backwardFlag = 0;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            duckFlag = 1;
            forwardFlag = 0;
            backwardFlag = 0;
        }
        else
        {
            duckFlag = 0;

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (invertGravity)
        {
            rigidBody.gravityScale = rigidBody.velocity.y < 0 ? 1f : gravityFall;
            rigidBody.gravityScale *= -1;
            spriteRenderer.flipY = true;
        }
        else
        {
            rigidBody.gravityScale = rigidBody.velocity.y > 0 ? 1f : gravityFall;
            spriteRenderer.flipY = false;
        }

        velocityVector2D.y = rigidBody.velocity.y;
        if (jumpFlag == 1)
        {
            //rigidBody.AddForce(Vector3.up*force);
            jumpCount++;
            if (jumpCount == 1)
            {
                if (invertGravity)
                {
                    velocityVector2D.y = -jumpVelocity;//rigidBody.velocity.y + jumpVelocity;
                }
                else
                {
                    velocityVector2D.y = jumpVelocity;//rigidBody.velocity.y + jumpVelocity;
                }
                   
            }
            else if (jumpCount == 2 && isDoubleJumpEnabled)
            {
                if (invertGravity)
                {
                    velocityVector2D.y = -doubleJumpVelocity;//rigidBody.velocity.y + jumpVelocity;
                }
                else
                {
                    velocityVector2D.y = doubleJumpVelocity;//rigidBody.velocity.y + jumpVelocity;
                }
            }

            isJumping = true;
            jumpFlag = 0;
        }
        //velocityVector2D.x = 0;

        if (duckFlag == 1)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        Debug.Log(forwardFlag);

        if (forwardFlag == 1)
        {
            if (velocityVector2D.x <= velocity)
            {
                velocityVector2D.x += velocityIncrement;
            }
            else
            {
                velocityVector2D.x = velocity;
            }

            if (velocityVector2D.x < 0)
            {
                m_Animator.SetBool("isSkidding", true);
            }
            else
            {
                m_Animator.SetBool("isSkidding", false);
            }
            
        }
        else if (backwardFlag == 1)
        {
            if (velocityVector2D.x >= -velocity)
            {
                velocityVector2D.x -= velocityIncrement;
            }
            else
            {
                velocityVector2D.x = -velocity;
            }


            if (velocityVector2D.x > 0)
            {
                m_Animator.SetBool("isSkidding", true);
            }
            else
            {
                m_Animator.SetBool("isSkidding", false);
            }
        }
        else
        {
            velocityVector2D.x = velocityVector2D.x - slowDown * velocityVector2D.x;
            //m_Animator.SetBool("isSkidding", true);
        }
        rigidBody.velocity = velocityVector2D;

        m_Animator.SetBool("isJumping", isJumping);
        m_Animator.SetBool("isCrouching", isCrouching);

        if (velocityVector2D.x < 0)
        {
            //transform.eulerAngles = new Vector3(0, 180, 0);
            spriteRenderer.flipX = true;
        }
        else
        {
            //transform.eulerAngles = new Vector3(0, 0, 0);
            spriteRenderer.flipX = false;
        }

        if (Mathf.Abs(velocityVector2D.x) > 0.5)
        {
            m_Animator.SetFloat("absSpeed", Mathf.Abs(velocityVector2D.x));
        }
        else
        {
            m_Animator.SetFloat("absSpeed", 0f);
            m_Animator.SetBool("isSkidding", false);
        }
        
        //rigidBody.rotation = 0f;
    }

    // called when the cube hits the floor
    void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("OnCollisionEnter2D");
        if (invertGravity)
        {
            if (col.gameObject.transform.position.y > rigidBody.transform.position.y)
            {
                jumpCount = 0;
                isJumping = false;
                //Debug.Log("Touch ground");
            }
        }
        else
        {
            if (col.gameObject.transform.position.y < rigidBody.transform.position.y)
            {
                jumpCount = 0;
                isJumping = false;
                //Debug.Log("Touch ground");
            }
        }
    }

    public void UpdateSize()
    {
        GetComponentInChildren<Animator>().SetInteger("marioSize", FindObjectOfType<LevelManager>().marioSize);
    }

}