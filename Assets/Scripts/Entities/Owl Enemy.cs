using System.Collections;
using UnityEngine;

public class OwlEnemy : MonoBehaviour, Hittable, Freezable {

    public Sprite owlIdle;
    
    public int owlEnemyDamage = 30;
    public float flyingForce = 5f;
    public float flyingUpwardsDuration = 3f;
    public float flyingIdleDuration = 2f;
    public float flyingDownwardsDuration = 3f;
    public float idleDuration = 3f;

    private float timer;

    private AnimatedSprite animatedSprite;
    private DeathAnimation deathAnimation;
    private SpriteRenderer spriteRenderer;

    private Vector3 restingPosition;
    private Vector3 activePosition;

    private bool freeze;
    private bool animating;
    private bool alive;

    private void Update() {
        if (!freeze && !animating) {
            timer -= Time.deltaTime;
            if (timer < 0) {
                timer = idleDuration;
                StartCoroutine(Animate());
            }
        }
    }

    private void Awake() {
        freeze = false;
        animating = false;
        alive = true;
        timer = idleDuration;
        animatedSprite = GetComponent<AnimatedSprite>();
        deathAnimation = GetComponent<DeathAnimation>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        restingPosition = transform.localPosition;
        activePosition = new Vector3(restingPosition.x, restingPosition.y + flyingForce, restingPosition.z);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.starPower || player.transform.DotTest(transform, Vector2.down)) {
                Hit(1000);
            } else {
                player.Hit(owlEnemyDamage);
            }
        }
    }

    public void Hit(int damage = 0) {
        // Even though this enemy won't fall in death barrier, just a little fail-safe
        if (damage != 100000) {
            GameManager.Instance.EnemyKilled();
        }
        Death();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit(1000);
        }
    }

    public void Freeze() {
        freeze = true;
        animatedSprite.enabled = true;
    }

    private void Death() {
        alive = false;
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
        enabled = true;
    }

    private void OnBecameInvisible() {
        GameManager instance= GameManager.Instance;
        if (instance != null) {
            instance.RemoveInvisibleEnemyFromList(gameObject);
        }
        enabled = false;
    }

    private IEnumerator Animate() {
        animating = true;
        animatedSprite.enabled = true;

        if (!freeze && alive) {
            yield return Move(restingPosition, activePosition, flyingUpwardsDuration);
        }

        yield return new WaitForSeconds(flyingIdleDuration);

        if (!freeze && alive) {
            yield return Move(activePosition, restingPosition, flyingDownwardsDuration);
        }

        animating = false;
        if (!freeze) {
            animatedSprite.enabled = false;
            spriteRenderer.sprite = owlIdle;
        } else {
            // When Owl is frozen mid-air, it looks little funny if idle sprite is loaded
            animatedSprite.enabled = true;
        }
    }

    private IEnumerator Move(Vector3 from, Vector3 to, float duration) {
        float elapsed = 0f;

        while (!freeze && alive && elapsed < duration) {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        if (!freeze && alive) {
            transform.localPosition = to;
        }
    }

}
