using System.Collections;
using UnityEngine;

public class BlockCollapsing : MonoBehaviour {

    public Sprite firstDamage;
    public Sprite secondDamage;
    public float collapseDuration = 1f;

    private SpriteRenderer spriteRenderer;
    private bool collapseStarted;

    private void Awake() {
        collapseStarted = false;
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (!collapseStarted && collider.CompareTag("Player")) {
            StartCoroutine(Animate());
        }
    }

    private IEnumerator Animate() {
        collapseStarted = true;
        // Immediately change to firstDamage sprite, so Player realises something is wrong
        spriteRenderer.sprite = firstDamage;
        yield return new WaitForSeconds(collapseDuration);
        spriteRenderer.sprite = secondDamage;
        yield return new WaitForSeconds(collapseDuration);
        gameObject.transform.parent.gameObject.SetActive(false);
    }

}
