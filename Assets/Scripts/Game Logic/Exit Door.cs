using System.Collections;
using UnityEngine;

public class ExitDoor : MonoBehaviour {

    private SpriteRenderer[] spriteRenderers;

    public Sprite openTopDoor;
    public Sprite openBottomDoor;

    // This value should represent value of next level, 1 is just default
    public int nextLevel = 1;
    
    private void Awake() {
        spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();
    }
    
    private void OnTriggerEnter2D(Collider2D collider) {
        // Only activate when Player touches exit door
        if (collider.CompareTag("Player")) {
            // Disable Players movement and movement of any entities surrounding Player
            collider.GetComponent<PlayerMovement>().enabled = false;
            GameManager.Instance.FreezeAllVisibleEnemies();

            // Just as a fail-safe if for some reason freeze doesn't work
            collider.GetComponent<Player>().setDamageInvincibility(3f);

            // Disable level timers
            GameManager.Instance.LevelBeingPlayed = false;

            // Start Door opening and Player leaving animation
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.PlaySFX("Win", 0.25f);
            StartCoroutine(Animate(collider));
        }
    }

    private IEnumerator Animate(Collider2D player) {
        yield return new WaitForSeconds(0.8f);
        spriteRenderers[1].sprite = openBottomDoor;
        spriteRenderers[2].sprite = openTopDoor;

        yield return new WaitForSeconds(0.8f);
        player.GetComponentInChildren<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(1f);
        GameManager.Instance.LoadLevelAfterDelay(10f, nextLevel, true, false, true);
    }

}
