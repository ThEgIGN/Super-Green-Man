using UnityEngine;

public class RollingEnemy : MonoBehaviour, Hittable, Freezable {

    public int rollingEnemyDamage = 100;
    public float rollingEnemySpeed = 2f;
    
    private new Collider2D collider2D;
    private new Rigidbody2D rigidbody2D;
    private AnimatedSprite animatedSprite;
    private SpriteRenderer spriteRenderer;
    private DeathAnimation deathAnimation;
    private Player player;
    private GameObject playerObject;

    private Vector2 velocity;
    private float direction;
    private bool flip;

    private bool followPlayer;

    private void Awake() {
        collider2D = GetComponent<Collider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animatedSprite = GetComponent<AnimatedSprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathAnimation = GetComponent<DeathAnimation>();
        playerObject = GameObject.FindWithTag("Player");
        player = playerObject.GetComponent<Player>();
        followPlayer = false;
    }

    private void Update() {
        if (followPlayer) {
            Movement();
        }
    }

    private void Movement() {
        // Change direction of movement based on position of Player
        if (playerObject.transform.position.x < transform.position.x) {
            direction = -1f;
            flip = true;
        } else {
            direction = 1f;
            flip = true;
        }

        velocity.x = Mathf.MoveTowards(velocity.x, direction * rollingEnemySpeed, rollingEnemySpeed * Time.deltaTime);
        velocity.y += Physics2D.gravity.y * Time.deltaTime;

        // Prevents gravity from building up while entity is grounded
        if (rigidbody2D.Raycast(spriteRenderer, false, Vector2.down)) {
            velocity.y = Mathf.Max(velocity.y, 0f);
        }

        if (flip) {
            Flip();
            flip = false;
        }
    }
    
    private void FixedUpdate() {
        rigidbody2D.MovePosition(rigidbody2D.position + velocity * Time.fixedDeltaTime);
    }

    private void Flip() {
        if (direction == -1f) {
            transform.eulerAngles = Vector3.zero;
        } else {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == playerObject) {
            if (player.starPower || collision.transform.DotTest(transform, Vector2.down)) {
                Hit(1000);
            } else {
                player.Hit(rollingEnemyDamage);
            }
        }
    }

    public void Hit(int damage = 0) {
        if (damage != 100000) {
            GameManager.Instance.EnemyKilled();
        }
        animatedSprite.enabled = false;
        deathAnimation.enabled = true;
        DisableEnemyAfterPeriod(3f);
    }

    private void OnBecameVisible() {
        GameManager instance= GameManager.Instance;
        if (instance != null) {
            instance.AddVisibleEnemyToList(gameObject);
        }
        enabled = true;
        followPlayer = true;
    }

    private void OnBecameInvisible() {
        GameManager instance= GameManager.Instance;
        if (instance != null) {
            instance.RemoveInvisibleEnemyFromList(gameObject);
        }
        enabled = false;
        followPlayer = false;
    }

    private void OnEnable() {
        rigidbody2D.WakeUp();
    }

    private void OnDisable() {
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.Sleep();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit(1000);
        }
    }

    public void Freeze() {
        enabled = false;
        followPlayer = false;
    }

    private void DisableEnemy() {
        gameObject.SetActive(false);
    }

    private void DisableEnemyAfterPeriod(float time) {
        Invoke(nameof(DisableEnemy), time);
    }

}
