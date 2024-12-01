using System.Collections;
using UnityEngine;

public class DeathAnimation : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    public Sprite deadSprite;

    private Collider2D [] colliders;
    private new Rigidbody2D rigidbody2D;
    private PlayerMovement playerMovement;
    private EntityMovement entityMovement;

    private void Reset() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake() {
        colliders = GetComponents<Collider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        entityMovement = GetComponent<EntityMovement>();
    }

    private void OnEnable() {
        UpdateSprite();
        DisablePhysics();
        StartCoroutine(Animate());
    }

    private void UpdateSprite() {
        spriteRenderer.enabled = true;
        // When entity dies, they need to render above everything on screen, so this needs to be big number
        spriteRenderer.sortingOrder = 15;

        if (deadSprite != null) {
            spriteRenderer.sprite = deadSprite;
        }
    }

    // Everything needs to be disabled, so when entity dies, they just "fall" through screen
    private void DisablePhysics() {
        // Disable all Collider2D (box, circle...)
        if (colliders != null) {
            foreach(Collider2D collider in colliders) {
                collider.enabled = false;
            }
        }

        // Disable Rigidbody2D
        if (rigidbody2D != null) {
            rigidbody2D.isKinematic = true;
        }

        // Disable either Players or Entities movement
        if (playerMovement != null) {
            playerMovement.enabled = false;
        }
        
        if (entityMovement != null) {
            entityMovement.enabled = false;
        }
    }

    // Allows for controlled animation at specific time periods with yield
    private IEnumerator Animate() {
        float elapsed = 0f;
        float duration = 3f;

        float jumpVelocity = 10f;
        float gravity = -36f;

        // How high entity "jumps" when they die
        Vector3 velocity = Vector3.up * jumpVelocity;

        while (elapsed < duration) {
            transform.position += velocity * Time.deltaTime;
            // How fast entity "falls" after jump when they die
            velocity.y += gravity * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

}
