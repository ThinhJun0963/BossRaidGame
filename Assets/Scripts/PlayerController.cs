using System.Collections;
using System.Collections.Generic;
using UnityEngine; // Contains Unity's core functionality
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f; // The speed of player
    private PlayerControls playerControls; // Represent the custom input actions defined in the Input System
    private Vector2 movement; // Stores the movement vector input from the player
    private Rigidbody2D rb; // References the 2D Rigidbody component attached to the player, enabling physics-based movement
    private Animator myAnimator; // Handles player animations like idle and movement animations
    private SpriteRenderer mySpriteRenderer; // Controls how the player's sprite is visually rendered, e.g., flipping it to face different directions.

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        PlayerInput();
        mySpriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRenderer.flipX = true;
        } else
        {
            mySpriteRenderer.flipX = false;
        }
    }
}
