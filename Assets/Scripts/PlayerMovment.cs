using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;       // Speed of horizontal movement
    public float jumpForce = 10f;      // Force applied for jumping
    public LayerMask groundLayer;      // Layer mask for ground detection
    private bool isGrounded = true;   // To check if the player is on the ground

    private Vector2 movementInput;     // To store horizontal input
    private Rigidbody2D rb;            // Reference to Rigidbody2D component
    private PlayerInput inputActions;

    [SerializeField] Transform groundCheck;
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position,0.2f,groundLayer);
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Initialize input actions
        inputActions = new PlayerInput();
    }

    private void OnEnable()
    {
        // Enable input actions
        inputActions.Player.Enable();

        // Subscribe to movement and jump actions
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        // Unsubscribe from input actions
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Jump.performed -= OnJump;

        // Disable input actions
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // Only jump if the player is grounded
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Handle horizontal movement (X axis only)
        rb.velocity = new Vector2(movementInput.x * moveSpeed, rb.velocity.y);

        // Check if the player is grounded
        isGrounded = IsGrounded();
    }
}
