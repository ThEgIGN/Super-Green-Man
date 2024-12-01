using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    
    private Rigidbody2D rigidBody;
    private Collider2D colliderPlayer;
    private SpriteRenderer spriteRenderer;
    private Player player;

    private Vector2 velocity;
    private float inputAxis;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;

    // If we were to use regular =, this property would be calculated only once
    // Whereas this acts as a get function and calculates it everytime from scratch
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);

    // Everyone can see these variables, but they can only be changed here
    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    // Player is running if character is moving or button is being held
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    // Player is sliding if button for direction is pressed while running in opposite direction
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
 
    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        colliderPlayer = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        player = GetComponent<Player>();
    }

    private void OnEnable() {
        rigidBody.isKinematic = false;
        colliderPlayer.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void OnDisable() {
        rigidBody.isKinematic = true;
        colliderPlayer.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update() {
        HorizontalMovement();

        // Check to see if there's ground below Player
        grounded = rigidBody.Raycast(spriteRenderer, false, Vector2.down);
        if (grounded) {
            GroundedMovement();
        }

        ApplyGravity();
    }

    private void HorizontalMovement() {
        inputAxis = Input.GetAxis("Horizontal");
        // Slowly increases velocity over time
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, moveSpeed * Time.deltaTime);

        if (rigidBody.Raycast(spriteRenderer, true, Vector2.right * velocity.x)) {
            velocity.x = 0f;
        }

        // Player is turned to right by default, so just reset when they are moving right
        if (velocity.x > 0f) {
            transform.eulerAngles = Vector3.zero;
        // If they are moving to the left, flip the Player by 180
        } else if (velocity.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        // If they are idle, leave model as it is
    }

    private void GroundedMovement() {
        // Prevents gravity from building up while Player is grounded
        velocity.y = Mathf.Max(velocity.y, 0f);
        // Player is jumping if they are going up
        jumping = velocity.y > 0f;
        
        if (Input.GetButtonDown("Jump")) {
            velocity.y = jumpForce;
            jumping = true;
            AudioManager.Instance.PlaySFX("Jump", 0.2f);
        }
    }

    private void ApplyGravity() {
        // Player starts falling when they are either going down or they stopped holding Jump button
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        // Gravity is stronger if Player is falling
        float multiplier = falling ? 2f : 1f;

        velocity.y += gravity * multiplier * Time.deltaTime;
        // Prevents Player from falling down too fast
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    // It's really good to tie anything related with physics to FixedUpdate, which runs at specific interval
    private void FixedUpdate() {
        // Increase current position by applying velocity over time
        Vector2 position = rigidBody.position;
        position += velocity * Time.fixedDeltaTime;
        rigidBody.MovePosition(position);
    }

    // This gets called when Player collides with object
    private void OnCollisionEnter2D(Collision2D collision) {
        int layer = collision.gameObject.layer;
        if (layer == LayerMask.NameToLayer("Enemy")) {
            // Player bounces from enemies heads
            AudioManager.Instance.PlaySFX("EnemyHit", 0.4f);
            if (transform.DotTest(collision.transform, Vector2.down)) {
                velocity.y = jumpForce;
                jumping = true;
            } else if (!player.starPower) {
                // Player took damage from enemy, they shouldn't keep momentum towards enemy
                // This will allow Player to immediately go backwards from enemy
                velocity.x = 0f;
            }
        }

        if (layer != LayerMask.NameToLayer("PowerUp")) {
            // If Player hits their head against block from any direction, they start falling immediately
            if (transform.DotTest(collision.transform, Vector2.up)) {
                velocity.y = 0f;
            }
        }
    }

    public void ApplyForceX(float force) {
        velocity.x = force;
    }

    public void ApplyForceY(float force) {
        velocity.y = force;
    }

}
