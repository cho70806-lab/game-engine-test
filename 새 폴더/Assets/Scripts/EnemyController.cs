using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private bool isMovingRight = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

  
    // Update is called once per frame
    void Update()
    {

        if (isMovingRight)
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Boundary"))
        {
            isMovingRight = !isMovingRight;
        }
    }
}
