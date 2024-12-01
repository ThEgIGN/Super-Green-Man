using UnityEngine;

public class RobotEnemy : MonoBehaviour, Hittable, Freezable {

    public int robotEnemyHealth = 300;
    public int robotEnemyDamage = 200;

    public float nextScale = 3f;
    public float speedIncrement = 0.5f;
    public float positionAdjustment = 0.2f;

    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private DeathAnimation deathAnimation;
    private AnimatedSprite animatedSprite;
    private EntityMovement entityMovement;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathAnimation = GetComponent<DeathAnimation>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animatedSprite = GetComponent<AnimatedSprite>();
        entityMovement = GetComponent<EntityMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.starPower) {
                Hit(1000);
            } else if (collision.transform.DotTest(transform, Vector2.down)) {
                Hit(50);
            } else {
                player.Hit(robotEnemyDamage);
            }
        }
    }

    public void Hit(int damage = 50) {
        if (damage == 100000) {
            Death();
        } else if (damage == 1000) {
            GameManager.Instance.EnemyKilled();
            Death();
        } else {
            robotEnemyHealth -= damage;
            if (robotEnemyHealth <= 0) {
                GameManager.Instance.EnemyKilled();
                Death();
            } else {
                ChangeVersion();
            }
        }
    }

    private void ChangeVersion() {
        transform.localScale = new Vector3(nextScale, nextScale, nextScale);
        nextScale -= 0.5f;

        entityMovement.speed = entityMovement.speed += speedIncrement;

        // // Because of the scale, next version spawns slighlty in the air, so this adjusts that
        transform.position = new Vector2(transform.position.x, transform.position.y - positionAdjustment);
    }

    private void Death() {
        animatedSprite.enabled = false;
        deathAnimation.enabled = true;
        DisableEnemyAfterPeriod(3f);
    }

    public void Freeze() {
        entityMovement.enabled = false;
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
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            GameManager.Instance.EnemyKilled();
            Death();
        }
    }

}
