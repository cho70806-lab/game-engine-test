using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator pAni;
    private bool isGrounded;
    private bool isGiant = false;

    private float moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pAni = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (isGiant)
        {
            if (moveInput < 0)
                transform.localScale = new Vector3(-2, 2, 2);
            else if (moveInput > 0)
                transform.localScale = new Vector3(2, 2, 2);
        }
        else
        {
            if (moveInput < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (moveInput > 0)
                transform.localScale = new Vector3(1, 1, 1);
        }

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 2f, groundLayer);
        
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
        if(collision.CompareTag("Respawn"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(collision.CompareTag("Finish"))
        {
            collision.GetComponent<LevelObject>().MoveToNextLevel();
        }

        if (collision.CompareTag("Enemy"))
        {
            if (isGiant)
                Destroy(collision.gameObject);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.CompareTag("item"))
        {
            isGiant = true;
            Invoke(nameof(ResetGiant), 3f);
            Destroy(collision.gameObject);
        }
    }

    void ResetGiant()
    {
        isGiant = false;
    }

}
