using System.Collections;
using UnityEngine;

public class BlockItem : MonoBehaviour {

    private Rigidbody2D rigidBody;
    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate() {
        enableEverything(false);

        // We want to wait for animation of block going up (because Player hit it)
        // before we start animation of item leaving it
        yield return new WaitForSeconds(0.25f);

        spriteRenderer.enabled = true;

        float elapsed = 0f;
        float duration = 0.5f;

        Vector3 startPos = transform.localPosition;
        Vector3 endPos = transform.localPosition + Vector3.up;

        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = endPos;
        
        // Now that animation of item going up is finished, re-enable physics of it so it can start moving
        enableEverything(true);
    }

    private void enableEverything(bool enable) {
        rigidBody.isKinematic = !enable;
        circleCollider.enabled = enable;
        boxCollider.enabled = enable;
        spriteRenderer.enabled = enable;
    }

}
