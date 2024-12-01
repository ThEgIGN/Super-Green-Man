using UnityEngine;

public class AnimatedSprite : MonoBehaviour {

    public Sprite[] sprites;
    public float frameRate = 1f / 6f;

    private SpriteRenderer spriteRenderer;
    private int frame;
    private int spritesLength;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spritesLength = sprites.Length;
    }

    private void OnEnable() {
        InvokeRepeating(nameof(Animate), frameRate, frameRate);
    }

    private void OnDisable() {
        CancelInvoke();
    }

    private void Animate() {
        frame++;
        if (frame >= spritesLength) {
            frame = 0;
        }

        if (frame >= 0 && frame < spritesLength)
        spriteRenderer.sprite = sprites[frame];
    }

}

