using UnityEngine;

public class ShellEnemy : MonoBehaviour, Hittable, Freezable {

    public Sprite shell;
    public int shellEnemyDamage = 50;
    public float shellSpeed = 12f;

    private bool inShell;
    private bool shellMoving;

    private new Rigidbody2D rigidbody2D;
    private EntityMovement entityMovement;
    private AnimatedSprite animatedSprite;
    private SpriteRenderer spriteRenderer;
    private DeathAnimation deathAnimation;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        entityMovement = GetComponent<EntityMovement>();
        animatedSprite = GetComponent<AnimatedSprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathAnimation = GetComponent<DeathAnimation>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Player should take damage when colliding with enemy not in their shell
        if (!inShell && collision.gameObject.CompareTag("Player")) {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.starPower) {
                Hit(1000);
            } else {
                if (collision.transform.DotTest(transform, Vector2.down)) {
                    EnterShell();
                } else {
                    player.Hit(shellEnemyDamage);
                }
            }
        }
    }

    // This triggers slightly bigger Box collider
    private void OnTriggerEnter2D(Collider2D collider) {
        if (inShell && collider.CompareTag("Player")) {
            Player player = collider.GetComponent<Player>();
            if (player.starPower) {
                Hit(1000);
            } else {
                // If Player collided with stopped shell, push it
                if (!shellMoving) {
                    Vector2 direction;
                    // Player jumped on it again. Send shell flying left or right
                    // depending on position of Player 
                    if (player.transform.DotTest(transform, Vector2.down)) {
                        if (player.transform.position.x > transform.position.x) {
                            direction = new Vector2(-1f, 0f);
                        } else {
                            direction = new Vector2(1f, 0f);
                        }
                    } else {
                        // Player pushed it from side. Adjust direction based on direction of push
                        direction = new Vector2(transform.position.x - collider.transform.position.x, 0f);
                    }
                    AudioManager.Instance.PlaySFX("EnemyHit", 0.4f);
                    PushShell(direction);
                } else {
                    // Big number just to make sure Player gets one-shot by this
                    player.Hit(500);
                }
            }
        // Moving shell insta-kills another regular shells
        } else if (collider.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit();
        }
    }

    public void Freeze() {
        entityMovement.enabled = false;
    }

    private void PushShell(Vector2 direction) {
        shellMoving = true;

        rigidbody2D.isKinematic = false;

        entityMovement.direction = direction;
        entityMovement.speed = shellSpeed;
        entityMovement.enabled = true;

        // By default, enemies don't collide with and damage other enemies
        // Adding moving shell to another layer helps with that
        gameObject.layer = LayerMask.NameToLayer("Shell");
    }

    private void EnterShell() {
        inShell = true;

        // When Player leaves shell off screen and comes back, thus re-enabling its movement,
        // shell needs to be idle, so that's why speed is set to 0f
        entityMovement.speed = 0f;
        entityMovement.enabled = false;

        animatedSprite.enabled = false;
        spriteRenderer.sprite = shell;
    }

    public void Hit(int damage = 0) {
        // Count towards Player kills only if they killed it, not Death Barrier
        if (damage != 100000) {
            GameManager.Instance.EnemyKilled();
        }

        animatedSprite.enabled = false;
        deathAnimation.enabled = true;
        DisableEnemyAfterPeriod(3f);
    }

    private void DisableEnemy() {
        gameObject.SetActive(false);
    }

    private void DisableEnemyAfterPeriod(float time) {
        Invoke(nameof(DisableEnemy), time);
    }
    
    private void OnBecameVisible() {
        GameManager instance= GameManager.Instance;
        if (instance != null) {
            instance.AddVisibleEnemyToList(gameObject);
        }
    }
    
    private void OnBecameInvisible() {
        GameManager instance= GameManager.Instance;
        if (instance != null) {
            instance.RemoveInvisibleEnemyFromList(gameObject);
        }

        // Remove Shell enemy once Player can't see it, so it doesn't spin around infinitely
        if (shellMoving) {
            DisableEnemy();
        }
    }

}

