using UnityEngine;

public class BlueEnemy : MonoBehaviour, Hittable, Freezable {

    public int blueEnemyHealth = 100;
    public int blueEnemyDamage = 50;
    
    public Sprite secondSprite;
    public Sprite flatSprite;
    public AnimatedSprite firstAnimatedSprite;
    public AnimatedSprite secondAnimatedSprite;

    private CapsuleCollider2D capsuleCollider2D;
    private EntityMovement entityMovement;
    private SpriteRenderer spriteRenderer;
    private DeathAnimation deathAnimation;

    private void Awake() {
        entityMovement = GetComponent<EntityMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathAnimation = GetComponent<DeathAnimation>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.starPower) {
                Hit(1000);
            } else if (collision.transform.DotTest(transform, Vector2.down)) {
                Hit(50);
            } else {
                player.Hit(blueEnemyDamage);
            }
        }
    }

    public void Hit(int damage = 100) {
        if (damage == 100000) {
            Death();
        } else if (damage == 1000) {
            GameManager.Instance.EnemyKilled();
            Death();
        } else {
            blueEnemyHealth -= damage;
            // If Player jumped on enemy for first time, change version. Else, flatten and kill enemy
            if (blueEnemyHealth <= 0) {
                GameManager.Instance.EnemyKilled();
                Flatten();
            } else {
                ChangeVersion();
            }
        }
    }

    private void ChangeVersion() {
        // Since second sprite is smaller, enemies scale and collider need to be adjusted
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        capsuleCollider2D.direction = CapsuleDirection2D.Horizontal;
        capsuleCollider2D.size = new Vector2(0.75f, 0.69f);

        spriteRenderer.sprite = secondSprite;
        firstAnimatedSprite.enabled = false;
        secondAnimatedSprite.enabled = true;

        // Because of the scale, second version spawns slighlty in the air, so this adjusts that
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
    }

    private void Flatten() {
        capsuleCollider2D.enabled = false;
        entityMovement.enabled = false;
        secondAnimatedSprite.enabled = false;
        spriteRenderer.sprite = flatSprite;
        DisableEnemyAfterPeriod(0.5f);
    }

    private void Death() {
        firstAnimatedSprite.enabled = false;
        secondAnimatedSprite.enabled = false;
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
            Hit(1000);
        }
    }

}
