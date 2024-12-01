using UnityEngine;

public class EntityMovement : MonoBehaviour {
    
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Vector2 velocity;

    public float speed = 2f;
    // Default movement will be left
    public Vector2 direction = Vector2.left;
    public bool shouldEntityFall;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enabled = false;
    }

    // Entities will start moving when Player actually sees them
    private void OnBecameVisible() {
        enabled = true;
    }

    // Entities will stop moving when Player can't see them
    private void OnBecameInvisible() {
        enabled = false;
    }

    private void OnEnable() {
        rigidBody.WakeUp();
    }

    private void OnDisable() {
        rigidBody.velocity = Vector2.zero;
        rigidBody.Sleep();
    }

    private void FixedUpdate() {
        velocity.x = direction.x * speed;
        velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;

        rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);

        // When they hit something left/right, flip
        if (rigidBody.Raycast(spriteRenderer, true, direction)) {
            direction = -direction;
        }

        bool grounded = rigidBody.Raycast(spriteRenderer, false, Vector2.down, direction.x /2);
        // Prevents gravity from building up while entity is grounded
        if (grounded) {
            velocity.y = Mathf.Max(velocity.y, 0f);
        // direction.x / 2
        } else if (!shouldEntityFall && !grounded){
            // Prevents entity from falling down. If offset raycast detected there's no block below, change direction
            direction = -direction;
        }

        Flip(direction.x);
    }

    private void Flip(float x) {
        // If entity is going left, return to default rotation, else flip it. 0f means idle 
        if (x > 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        } else if (x < 0f) {
            transform.eulerAngles = Vector3.zero;
        }
    }

}
