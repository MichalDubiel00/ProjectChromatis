using UnityEngine;

public class SimplePlayerMovment : MonoBehaviour
{
    public float moveSpeed = 5f;      // Horizontal movement speed
    public float jumpForce = 10f;     // Jump force

    private Rigidbody2D rb;           // Rigidbody2D component
    private bool isGrounded = false;  // To check if the player is on the ground
    [SerializeField] private GameInput gameInput;      // Reference to GameInput instance
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private LayerMask _groundLayer;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Horizontal movement
        Vector2 movementInput = gameInput.GetMovmentInput();
        rb.velocity = new Vector2(movementInput.x * moveSpeed, rb.velocity.y);

        // Jump
        if (gameInput.GetJump() && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Ground check using raycast
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            isGrounded = true;
        else
            isGrounded = false;
            Debug.DrawRay(transform.position, Vector2.down * 1.1f, Color.red);
    }
}
