using UnityEngine;

public class CloudEnemy : MonoBehaviour, Hittable, Freezable {

    public int cloudEnemyDamage = 20;
    public float cloudEnemySpeed = 0.01f;

    private Player player;
    private GameObject playerObject;
    private AnimatedSprite animatedSprite;
    private DeathAnimation deathAnimation;

    private bool movingTowardsPlayer;

    private void Awake() {
        movingTowardsPlayer = false;
        playerObject = GameObject.FindWithTag("Player");
        player = playerObject.GetComponent<Player>();
        animatedSprite = GetComponent<AnimatedSprite>();
        deathAnimation = GetComponent<DeathAnimation>();
    }
    
    // Every frame, Cloud enemy tracks players position and tries to get closer
    // Originally this was in Update, but after playing game on stronger computer,
    // Update was called so many times to the point that Cloud Enemy would instantly teleport to player
    // So FixedUpdate fixes that
    private void FixedUpdate() {
        if (movingTowardsPlayer) {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, cloudEnemySpeed);
        }
    }

    public void Freeze() {
        movingTowardsPlayer = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject == playerObject) {
            // If Player is in star power mode or they jumped on enemy, they should kill it
            if (player.starPower || collision.transform.DotTest(transform, Vector2.down)) {
                Hit(1000);
            } else {
                // This means Player collided with enemy from any other direction and should take damage
                player.Hit(cloudEnemyDamage);
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
        movingTowardsPlayer = true;
    }

    private void OnBecameInvisible() {
        GameManager instance= GameManager.Instance;
        if (instance != null) {
            instance.RemoveInvisibleEnemyFromList(gameObject);
        }
        enabled = false;
        movingTowardsPlayer = false;
    }

    private void DisableEnemy() {
        gameObject.SetActive(false);
    }

    private void DisableEnemyAfterPeriod(float time) {
        Invoke(nameof(DisableEnemy), time);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit(1000);
        }
    }

}
