using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class Basic2DMovement : MonoBehaviour
{
    //Variables important to the player moving.
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    
    //Variables important to the player jumping.
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private BoxCollider2D groundedChecker;
    public bool isGrounded = true;

    //Variables to allow jump buffering.
    [Header("Jump Buffer Settings")]
    [SerializeField] private float jumpBufferTimer;
    private float originalJumpBufferTimer;
    bool jumpBuffered = false;
    bool hasJumped = false;

    [Header("Fast Fall Properties")]
    [SerializeField] private float fastFallSpeed;
    bool fastFallActive = false;
    private float originalGravityScale;
    [SerializeField] private float yVelocity;

    [Header("Coyote Time Settings")]
    [SerializeField] private float coyoteTimer;
    private float originalCoyoteTimer;
    bool coyoteTime = true;

    [Header("Dash Variables")]
    public float dashLength;
    public float dashDistance;
    public bool canDash = true;
    public bool isDashing = false;

    private IEnumerator dashCorotine;

    [Header("Energy Settings")]
    public int energyAmount = 3;
    public bool hasEnergy;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        originalJumpBufferTimer = jumpBufferTimer;
        originalCoyoteTimer = coyoteTimer;
        originalGravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        //Basic Movement Logic
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(horizontalAxis * moveSpeed, rb.linearVelocity.y);
            //Jump Logic
            CheckIfJumpIsBuffered();
            CheckIfCoyoteTime();
        }
        
        yVelocity = rb.linearVelocity.y;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffered = true;
        }
        
        if ((coyoteTime && jumpBuffered) || (isGrounded && jumpBuffered))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpBufferTimer = 0;
            coyoteTimer = 0;
            isGrounded = false;
            hasJumped = true;
        }

        // Energy Jumps
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if ((energyAmount >= 1 && fastFallActive) || (energyAmount >= 1 && !isGrounded && !hasJumped))
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
                energyAmount -= 1;
            }
        }

        if (hasJumped && isGrounded)
        {
            hasJumped = false;
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (isGrounded == false)
            {
                fastFallActive = true;
            }
        }

        // Dash

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && energyAmount >= 1)
        {
            print("Dash Activated!");
            dashCorotine = TimedDash(dashLength);
            StartCoroutine(dashCorotine);
            energyAmount -= 1;
        }

        FastFall();
    }

    private IEnumerator TimedDash(float dashTime)
    {
        isDashing = true;
        canDash = false;
        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.linearVelocity = new Vector2(dashDistance * horizontalAxis, rb.linearVelocityY);

        yield return new WaitForSeconds(dashTime);

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isDashing = false;
        canDash = true;
    }

    void FastFall()
    {
        if (fastFallActive)
        {
            if (isGrounded == false)
            {
                rb.gravityScale = fastFallSpeed;
            }
        }
    }

    void CheckIfJumpIsBuffered()
    {
        if (jumpBuffered)
        {
            jumpBufferTimer -= Time.deltaTime;
            if (jumpBufferTimer <= 0)
            {
                jumpBuffered = false;
            }
        }
        else if (jumpBuffered == false)
        {
            jumpBufferTimer = originalJumpBufferTimer;
        }
    }
    
    void CheckIfCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimer = originalCoyoteTimer;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        if (coyoteTimer > 0)
        {
            coyoteTime = true;
        }
        else
        {
            coyoteTime = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.gravityScale = originalGravityScale;
            fastFallActive = false;
        }

        if (other.CompareTag("Energy"))
        {
            energyAmount = 3;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
