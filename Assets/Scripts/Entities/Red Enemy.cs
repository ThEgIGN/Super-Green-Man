using UnityEngine;

public class RedEnemy : MonoBehaviour, Hittable, Freezable {

    public Sprite flatRedEnemy;
    public int redEnemyDamage = 50;

    private bool killedByShell;

    private new Collider2D collider2D;
    private EntityMovement entityMovement;
    private AnimatedSprite animatedSprite;
    private SpriteRenderer spriteRenderer;
    private DeathAnimation deathAnimation;

    private void Awake() {
        killedByShell = false;
        collider2D = GetComponent<Collider2D>();
        entityMovement = GetComponent<EntityMovement>();
        animatedSprite = GetComponent<AnimatedSprite>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        deathAnimation = GetComponent<DeathAnimation>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Player player = collision.gameObject.GetComponent<Player>();
            // If Player is in star power mode, they should insta-kill enemies
            if (player.starPower) {
                Hit(1000);
            // Check if Player collided with enemy while going down, aka jumped on them
            } else if (collision.transform.DotTest(transform, Vector2.down)) {
                Flatten();
            } else {
                // This means Player collided with Red Enemy from any other direction and should take damage
                player.Hit(redEnemyDamage);
            }
        }
    }

    public void Freeze() {
        entityMovement.enabled = false;
    }

    // Red enemy should be insta-killed by moving shell
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            killedByShell = true;
            Hit();
        }
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

    private void Flatten() {
        GameManager.Instance.EnemyKilled();
        collider2D.enabled = false;
        entityMovement.enabled = false;
        animatedSprite.enabled = false;
        spriteRenderer.sprite = flatRedEnemy;
        DisableEnemyAfterPeriod(0.5f);
    }

    public void Hit(int damage = 0) {
        // Count towards Player kills only if they killed it, not Death Barrier
        if (damage != 100000) {
            GameManager.Instance.EnemyKilled();
        }
        // Flips enemy upside-down when they die by shell
        if (killedByShell) {
            transform.eulerAngles = new Vector3(180f, 0f, 0f);
        }
        animatedSprite.enabled = false;
        deathAnimation.enabled = true;
        DisableEnemyAfterPeriod(3f);
    }

    private void DisableEnemy() {
        // Reason for not using Destroy is because it calls Garbage Collector every single time
        gameObject.SetActive(false);
    }

    private void DisableEnemyAfterPeriod(float time) {
        Invoke(nameof(DisableEnemy), time);
    }

}
