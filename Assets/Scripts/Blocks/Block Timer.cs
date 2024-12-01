using UnityEngine;

public class BlockTimer : MonoBehaviour {

    // Timer for when should block change its state
    public float timerDurationChange = 5f;
    // Timer for how long should block stay enabled before disabling itself
    public float timerDurationDisable = 5f;
    public bool isActive;
    private float timerChange;
    private float timerDisable;
    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    
    private void Awake() {
        timerChange = timerDurationChange;
        timerDisable = timerDurationDisable;
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enableState();
    }

    private void Update() {
        timerChange -= Time.deltaTime;
        timerDisable -= Time.deltaTime;
        // Switch state of block when timer hits zero and restart timer
        if (timerChange < 0) {
            timerChange = timerDurationChange;
            timerDisable = timerDurationDisable;
            isActive = !isActive;
            enableState();
        } else if (isActive && timerDisable < 0) {
            timerDisable = timerDurationDisable;
            isActive = !isActive;
            enableState();
        }
    }

    private void enableState() {
        boxCollider2D.enabled = isActive;
        spriteRenderer.enabled = isActive;
    }

}
