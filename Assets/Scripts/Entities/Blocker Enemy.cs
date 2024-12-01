using System.Collections;
using UnityEngine;

public class BlockerEnemy : MonoBehaviour, Freezable {

    public int blockerEnemyDamage = 500;
    public Sprite sadBlocker, madBlocker;
    public float raycastDistance = 10f;
    public float raycastOffset = 0.5f;
    public string directionString;
    public float goingToPlayerDuration = 1f;
    public float goingBackDuration = 3f;

    private bool freeze;
    private bool animating;
    private Vector3 restingPosition;
    private Vector3 activePosition;
    private Vector2 direction;
    private float xBlockerOffset;

    private SpriteRenderer spriteRenderer;

    private LayerMask playerLayer;

    private void Awake() {
        freeze = false;
        animating = false;

        ConfigureDirection();
        
        restingPosition = transform.localPosition;
        playerLayer = LayerMask.GetMask("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void ConfigureDirection() {
        switch (directionString) {
            case "up":
                activePosition = new Vector3(transform.position.x, transform.position.y + raycastDistance, 0f);
                xBlockerOffset = raycastOffset;
                direction = Vector2.up;
                break;
            case "left":
                activePosition = new Vector3(transform.position.x - raycastDistance, transform.position.y, 0f);
                xBlockerOffset = 0f;
                direction = Vector2.left;
                break;
            case "right":
                activePosition = new Vector3(transform.position.x + raycastDistance, transform.position.y, 0f);
                xBlockerOffset = 0f;
                direction = Vector2.right;
                break;
            default:
                activePosition = new Vector3(transform.position.x, transform.position.y - raycastDistance, 0f);
                xBlockerOffset = raycastOffset;
                direction = Vector2.down;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {
            Player player = collider.GetComponent<Player>();
            player.Hit(blockerEnemyDamage);
        }
    }

    public void Freeze() {
        freeze = true;
    }

    private bool Raycast() {
        // Checks left and right from blocker with slight offset if blocker is going up/down
        // If blocker is going left/right, checks with no offset
        Vector2 leftPosition = new Vector2(restingPosition.x - xBlockerOffset, restingPosition.y);
        Vector2 rightPosition = new Vector2(restingPosition.x + xBlockerOffset, restingPosition.y);

        float radius = 0.25f;

        RaycastHit2D hit1 = Physics2D.CircleCast(leftPosition, radius, direction.normalized, raycastDistance, playerLayer);
        RaycastHit2D hit2 = Physics2D.CircleCast(rightPosition, radius, direction.normalized, raycastDistance, playerLayer);

        return hit1.collider != null || hit2.collider != null;
    }

    private void FixedUpdate() {
        if (!animating && Raycast()) {
            StartCoroutine(Animate());
        }
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
        if (!freeze) {
            yield return Move(restingPosition, activePosition, goingToPlayerDuration);
            AudioManager.Instance.PlaySFX("BlockHit", 0.5f);
        }
        if (!freeze) {
            spriteRenderer.sprite = sadBlocker;
            yield return Move(activePosition, restingPosition, goingBackDuration);
            spriteRenderer.sprite = madBlocker;
        }
        animating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to, float duration) {
        float elapsed = 0f;

        while (!freeze && elapsed < duration) {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        if (!freeze) {
            transform.localPosition = to;
        }
    }

}
