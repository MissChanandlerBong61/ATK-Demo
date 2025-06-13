//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//public class PlayerController : MonoBehaviour
//{
//    public int difficulty;

//    public float moveSpeed = 5.0f;
//    public float jumpRange = 10.0f;
//    public float dashRange = 10.0f;
//    public float climbspeed = 3.0f;
//    public Vector3 tieOffset = new Vector3(-0.047f, 0.629f, 0f);
//    public int maxHealth = 6;
//    int currentHealth;

//    public Transform groundCheck;
//    public float groundCheckRadius;
//    public LayerMask groundLayer;
//    public LayerMask fallLayer;
//    public LayerMask waterLayer;    
//    public LayerMask wallLayer;
//    public LayerMask enemyLayer;
    

//    [Header("Tie System")]
//    public GameObject tiePrefab;
//    private GameObject currentTie;
//    public float tieSpeed = 20.0f;
//    public float maxtieLength = 50.0f;

//    private GunSystem gunSystem;
//    private Animator playerAnimator;
//    private Vector2 moveDirection = new Vector2(1, 0);
//    private Rigidbody2D playerRb;
//    private bool isGrounded;
//    private bool isClimbing = false;        
//    private float horizontalInput;
//    private float verticalInput;
    
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        playerAnimator = GetComponent<Animator>();
//        playerRb = GetComponent<Rigidbody2D>();
//        gunSystem = GetComponent<GunSystem>();
//        difficulty = PlayerPrefs.GetInt("difficulty");

//        int difficultyResult = difficulty;

//        dashRange /= difficultyResult;

//        currentHealth = maxHealth;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (GameManager.Instance != null && GameManager.Instance.isGamePaused) return;
//        horizontalInput = Input.GetAxisRaw("Horizontal");
//        verticalInput = Input.GetAxisRaw("Vertical");
        
//        if (!isClimbing)
//        {
//            Vector2 movementInput = new Vector2(horizontalInput * moveSpeed, playerRb.linearVelocityY);
//            playerRb.linearVelocity = movementInput;
//        }
//        else
//        {
//            playerRb.linearVelocity = new Vector2 (horizontalInput * moveSpeed, verticalInput * climbspeed);
//            playerRb.gravityScale = 0;
//        }
        
//        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);



//        playerAnimator.SetFloat("Look X", horizontalInput);
//        playerAnimator.SetFloat("Look Y", verticalInput);
//        playerAnimator.SetFloat("Speed", Mathf.Abs(horizontalInput));



//        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
//        {
//            playerRb.AddForce(Vector2.up * jumpRange, ForceMode2D.Impulse);
//            //playerAnimator.SetBool("Jump", true);
//        }

//        if (Input.GetKeyDown(KeyCode.LeftControl))
//        {
//            Dash();
//        }

//        if (gunSystem != null && Input.GetMouseButtonDown(0))
//        {
//            gunSystem.TryShoot();
//        }
//        if (Input.GetMouseButtonDown(1))
//        {
//            FireWhip();
//        }
//        if (Input.GetMouseButtonUp(1) && currentTie != null)
//        {
//            currentTie.GetComponent<TieMechanics>().Retract();
//        }

//        FallWater();
//    }

//    private void OnDrawGizmos()
//    {
//        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
//    }  

//    private void Dash()
//    {
//        Vector2 dashDirection = new Vector2(horizontalInput, 0f).normalized;
//        playerRb.linearVelocity = dashDirection*dashRange;
//    }
//    private void FireWhip()
//    {
//        if (currentTie != null) return;

               
//        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//        currentTie = Instantiate(tiePrefab, transform.position + tieOffset, Quaternion.identity);
        

//        var tieScript = currentTie.GetComponent<TieMechanics>();
//        if (tieScript.tieVisual != null)
//        {
//            var sr = tieScript.tieVisual.GetComponent<SpriteRenderer>();
//            if (sr != null)
//            {
//                float r = PlayerPrefs.GetFloat("TieColor_R", 1f);
//                float g = PlayerPrefs.GetFloat("TieColor_G", 1f);
//                float b = PlayerPrefs.GetFloat("TieColor_B", 1f);
//                float a = PlayerPrefs.GetFloat("TieColor_A", 1f);
//                sr.color = new Color(r, g, b, a);
//            }
//        }
//        tieScript.StartWhip(transform.position + tieOffset, target, playerRb);
//        tieScript.OnRetracted += () => currentTie = null;        
//    }

//    public void ChangeHealth(int amount)
//    {
//        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

//    }

//    public void TakeDamage()
//    {
//        playerAnimator.SetBool("Hit", true);
//        playerRb.angularVelocity /= 2;
//        Invoke("ResetHit", 0.5f); 
//    }

//    void ResetHit()
//    {
//        playerAnimator.SetBool("Hit", false);
//    }

//    private void FallWater()
//    {
//        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, waterLayer))
//        {
//            Debug.Log("Az Dikkat Laððð");
//            transform.position = new Vector3((transform.position.x - 2.0f), (transform.position.y + 3), 0);
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.gameObject.CompareTag("Ladder") && verticalInput != 0)
//        {
//            isClimbing = true;
//        }

//    }
//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.gameObject.CompareTag("Ladder"))
//        {
//            isClimbing = false;
//            playerRb.gravityScale = 1f;
//        }
//    }
//}
