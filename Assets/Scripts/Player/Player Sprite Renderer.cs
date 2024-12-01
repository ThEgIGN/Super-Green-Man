using UnityEngine;

public class PlayerSpriteRenderer : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;

    public AnimatedSprite idle;
    public Sprite jump;
    public Sprite slide;
    public AnimatedSprite run;
    
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void OnEnable() {
        spriteRenderer.enabled = true;
    }

    private void OnDisable() {
        // When Player died, one of these stayed enabled, so death animation wasn't playing properly
        idle.enabled = false;
        run.enabled = false;
        
        spriteRenderer.enabled = false;
    }
    
    // Sprite changing is also good idea to do in LateUpdate
    private void LateUpdate() {
        run.enabled = playerMovement.running;
        idle.enabled = !playerMovement.running;

        if (playerMovement.jumping) {
            spriteRenderer.sprite = jump;
        } else if (playerMovement.sliding) {
            spriteRenderer.sprite = slide;
        }
    }

}
