using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("이동 및 점프")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private SpriteRenderer spriteRenderer; // 캐싱 추가
    private bool isGrounded;
    private float moveInput;

    // 아이템 상태 및 원래 능력치 저장 변수
    private bool isGiant = false;
    private bool isInvincible = false;
    private float originalSpeed;
    private float originalJumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Awake에서 미리 가져오기

        originalSpeed = moveSpeed;
        originalJumpForce = jumpForce;
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // --- 애니메이션 처리 (추가) ---
        // moveInput이 0이 아니면 달리는 중(true), 0이면 정지(false)
        bool isMoving = moveInput != 0;
        pAni.SetBool("isRunning", isMoving);

        // 거대화 상태에 따른 스케일 결정
        if (isGiant)
        {
            if (moveInput < 0) transform.localScale = new Vector3(-2, 2, 2);
            else if (moveInput > 0) transform.localScale = new Vector3(2, 2, 2);
        }
        else
        {
            if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
            else if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        pAni.SetBool("isGrounded", isGrounded); 
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            pAni.SetTrigger("Jump");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }

        if (collision.CompareTag("Enemy"))
        {
            {

                // 거대화 상태이거나 무적 상태라면 적을 파괴함

                if (isGiant || isInvincible)

                {

                    Destroy(collision.gameObject);

                }

                else

                {

                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                }

            }
        }

        // 아이템 로직들
        if (collision.CompareTag("item"))
        {
            isGiant = true;
            CancelInvoke(nameof(ResetGiant));
            Invoke(nameof(ResetGiant), 6f);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Speeditem"))
        {
            moveSpeed = originalSpeed * 2f;
            CancelInvoke(nameof(ResetSpeed));
            Invoke(nameof(ResetSpeed), 3f);
            Destroy(collision.gameObject);
            pAni.speed = 2f;
        }

        if (collision.CompareTag("Jumpitem"))
        {
            jumpForce = originalJumpForce * 1.5f;
            CancelInvoke(nameof(ResetJump));
            Invoke(nameof(ResetJump), 5f);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Staritem"))
        {
            isInvincible = true;
            spriteRenderer.color = Color.yellow;
            CancelInvoke(nameof(ResetInvincible));
            Invoke(nameof(ResetInvincible), 5f);
            Destroy(collision.gameObject);
        }
    }

    void ResetGiant() 
    { isGiant = false; }

    void ResetSpeed() 
    { moveSpeed = originalSpeed; }

    void ResetJump() 
    { jumpForce = originalJumpForce; }

    void ResetInvincible()
    {
        isInvincible = false;
        spriteRenderer.color = Color.white;
    }
}