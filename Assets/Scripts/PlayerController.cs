using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField]
    float fJumpVelocity = 5;

    float fJumpPressedRemember = 0;
    [SerializeField]
    float fJumpPressedRememberTime = 0.2f;

    float fGroundedRemember = 0;
    [SerializeField]
    float fGroundedRememberTime = 0.25f;
    // [SerializeField]
    // float fHorizontalAcceleration = 1;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingBasic = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenStopping = 0.5f;
    [SerializeField]
    [Range(0, 1)]
    float fHorizontalDampingWhenTurning = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    float fCutJumpHeight = 0.5f;


    public Rigidbody2D theRB;

    [Header("Manual Speed")]

    private float m_moveSpeed;
    private float m_acceleration;
    private float m_deceleration;
    private bool m_enableManualVelocityControl = false;
    private float _velocity;


    [Header("Jump")]

    private float jumpForce;
    private bool m_enableJumpDampen;
    private float _jumpVelocity;

    private float JumpDampening=0.1f;   
 
    public Transform groundCheckPoint;
    public LayerMask m_isGrounded;

    private bool isGrounded;

    private bool canDoubleJump;

    [Header("Control")]
    public bool stopInput;

    [Header("Knockback")]
    public float knockBackLength, knockBackForce;
    public float bounceForce;
    private float knockBackCounter;

    private Animator anim;
    private SpriteRenderer theSR;

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
                Vector2 v2GroundedBoxCheckPosition = (Vector2)transform.position + new Vector2(0, -0.01f);
                Vector2 v2GroundedBoxCheckScale = (Vector2)transform.localScale + new Vector2(-0.02f, 0);
                isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, .2f, m_isGrounded);

                fGroundedRemember -= Time.deltaTime;
                if (isGrounded)
                {
                    fGroundedRemember = fGroundedRememberTime;
                }

                fJumpPressedRemember -= Time.deltaTime;
                if (Input.GetButtonDown("Jump"))
                {
                    fJumpPressedRemember = fJumpPressedRememberTime;
                }

                if (Input.GetButtonUp("Jump"))
                {
                    if (theRB.velocity.y > 0)
                    {
                        theRB.velocity = new Vector2(theRB.velocity.x, theRB.velocity.y * fCutJumpHeight);
                    }
                }

                if ((fJumpPressedRemember > 0) && (fGroundedRemember > 0))
                {
                    fJumpPressedRemember = 0;
                    fGroundedRemember = 0;
                    theRB.velocity = new Vector2(theRB.velocity.x, fJumpVelocity);
                }

                float fHorizontalVelocity = theRB.velocity.x;
                fHorizontalVelocity += Input.GetAxisRaw("Horizontal");

                // if (fHorizontalVelocity != 0.0f) {
                //     Debug.Log("velocity: " + fHorizontalVelocity + " dT: " + Time.deltaTime);
                // }

                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.01f)
                    fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenStopping, Time.deltaTime * 10f);
                else if (Mathf.Sign(Input.GetAxisRaw("Horizontal")) != Mathf.Sign(fHorizontalVelocity))
                    fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingWhenTurning, Time.deltaTime * 10f);
                else
                    fHorizontalVelocity *= Mathf.Pow(1f - fHorizontalDampingBasic, Time.deltaTime * 10f);

                theRB.velocity = new Vector2(fHorizontalVelocity, theRB.velocity.y);

                // UpdateHorizontalPosition();
                // DoJump();

                // flip the object based on their movement direction
                if (m_enableManualVelocityControl && _velocity < 0 || theRB.velocity.x < 0)
                {
                    theSR.flipX = true;
                }
                else if (m_enableManualVelocityControl && _velocity > 0 || theRB.velocity.x > 0)
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

        anim.SetFloat("moveSpeed", (m_enableManualVelocityControl) 
                                        ? Mathf.Abs(_velocity)
                                        : Mathf.Abs(theRB.velocity.x));
        anim.SetBool("isGrounded", isGrounded);
    }

    private void DoJump()
    {
        
        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {   
                if (m_enableJumpDampen) {
                    // manual position control for dampen
                    _jumpVelocity = jumpForce;
                } else {
                    theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
                }
                AudioManager.instance.PlaySFX(10);
            }
            else
            {
                if (canDoubleJump)
                {
                    if (m_enableJumpDampen) {
                        theRB.velocity = new Vector2(theRB.velocity.x, jumpForce);
                    } else {
                        _jumpVelocity = jumpForce;
                    }
                    canDoubleJump = false;
                    AudioManager.instance.PlaySFX(10);
                }
            }
        }
    }

    // private void UpdateJumpMovement() {
    //      Vector3 pos = transform.position;
         
    //      if (_jumpVelocity != 0)
    //      {
    //          pos.y += new Vector3(0f,JumpVelocity, 0f);
    //          JumpVelocity -= JumpDampening;
    //          if (JumpVelocity <= 0)
    //          {
    //              gameObject.rigidbody.useGravity = true;
    //              JumpVelocity = 0;
    //          }
    //      }
 
    //      transform.position = pos;
    // }

    private void UpdateHorizontalPosition()
    {     
        if (!m_enableManualVelocityControl) {
            theRB.velocity = new Vector2(m_moveSpeed * Input.GetAxis("Horizontal"), theRB.velocity.y);
            return;
        }


        // var horizontalInput = Input.GetAxis("Horizontal");

        if ((Input.GetKey("left")) && (_velocity < m_moveSpeed)) 
        // if (horizontalInput < 0 && (_velocity < m_moveSpeed)) 
            _velocity = _velocity - m_acceleration * Time.deltaTime;
        else if ((Input.GetKey("right")) && (_velocity > -m_moveSpeed)) 
        // else if (horizontalInput > 0 && (_velocity > -m_moveSpeed)) 
            _velocity = _velocity + m_acceleration * Time.deltaTime;

        else
        {
            if (_velocity > m_deceleration * Time.deltaTime) 
                _velocity = _velocity - m_deceleration * Time.deltaTime;
            else if (_velocity < -m_deceleration * Time.deltaTime) 
                _velocity = _velocity + m_deceleration * Time.deltaTime;
            else
                _velocity = 0;
        }

        // stop updating velocity on death
        if (PlayerHealthController.instance.maxHealth <= 0)
            _velocity = 0;

        transform.position += new Vector3(_velocity * Time.deltaTime, 0f, 0f);
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
}
