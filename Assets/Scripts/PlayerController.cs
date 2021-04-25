using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;


    [Header("Movement")]

    public float m_moveSpeed;
    public Rigidbody2D theRB;

    [Header("Jumping")]

    public float jumpForce;


    public Transform groundCheckPoint;
    public LayerMask m_isGrounded;

    private bool isGrounded;


    private bool canDoubleJump;

    private Animator anim;
    private SpriteRenderer theSR;

    public float knockBackLength, knockBackForce;
    private float knockBackCounter;

    public float bounceForce;

    public bool stopInput;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        theSR = GetComponent<SpriteRenderer>();
    }




    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.instance.isPaused && !stopInput)
        {
            if (knockBackCounter <= 0)
            {
                theRB.velocity = new Vector2(m_moveSpeed * Input.GetAxis("Horizontal"), theRB.velocity.y);

                isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, m_isGrounded);

                if (isGrounded)
                {
                    canDoubleJump = true;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    _DoJump();
                }

                if (Input.GetKeyDown(KeyCode.Q) && 
                         LayerManager.instance.CanTransitionLayer(transform.position, towards: true)) {
                    _DoJump();
                    transform.position -= new Vector3(0,0, LayerManager.instance.depthUnit);
                    Debug.LogFormat("[{0}] You pressed Q! (z={1})", CameraController.Clock, transform.position.z);
                    LayerManager.instance.onLayerTransition(transform.position.z);
                }
                else if (Input.GetKeyDown(KeyCode.E) &&
                         LayerManager.instance.CanTransitionLayer(transform.position, towards: false)) {
                    _DoJump();
                    transform.position += new Vector3(0, 0, LayerManager.instance.depthUnit);
                    Debug.LogFormat("[{0}] You pressed E! (z={1})", CameraController.Clock, transform.position.z);
                    LayerManager.instance.onLayerTransition(transform.position.z);
                }

                if (theRB.velocity.x < 0)
                {
                    theSR.flipX = true;
                }
                else if (theRB.velocity.x > 0)
                {
                    theSR.flipX = false;
                }
            }
            else
            {
                knockBackCounter -= Time.deltaTime;
                if (!theSR.flipX)
                {
                    theRB.velocity = new Vector2(-knockBackForce, theRB.velocity.y);
                }
                else
                {
                    theRB.velocity = new Vector2(knockBackForce, theRB.velocity.y);
                }
            }
        }

        anim.SetFloat("moveSpeed", Mathf.Abs( theRB.velocity.x));
        anim.SetBool("isGrounded", isGrounded);
    }

    private void _DoJump()
    {
        if (isGrounded)
        {
            theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
            AudioManager.instance.PlaySFX(10);
        }
        else
        {
            if (canDoubleJump)
            {
                theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
                canDoubleJump = false;
                AudioManager.instance.PlaySFX(10);
            }
        }
    }

    public void KnockBack()
    {
        knockBackCounter = knockBackLength;
        theRB.velocity = new Vector2(0f, knockBackForce);

        anim.SetTrigger("hurt");
    }

    public void Bounce()
    {
        theRB.velocity = new Vector2(theRB.velocity.x, bounceForce);
        AudioManager.instance.PlaySFX(10);
    }

    public bool HasJumped() {
        // Debug.LogFormat("[{0}] playerController: isGrounded: {1} velocity.y: {2}", 
        //                 CameraController.instance.Clock, isGrounded, theRB.velocity.y);
        bool res = !isGrounded && theRB.velocity.y > 0.0f;

        return res;
    }
}
