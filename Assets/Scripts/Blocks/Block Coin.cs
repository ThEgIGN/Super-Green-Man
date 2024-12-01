using System.Collections;
using UnityEngine;

public class BlockCoin : MonoBehaviour {

    // We need to set Sprites, chances of each one happening, and so on
    // Important!!! Set them in ascending order, last one doesn't need chance
    public Sprite[] sprites;
    public float[] chances;
    public int[] coinValues;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        ChooseRandomCoin();
        StartCoroutine(Animate());
    }

    private void ChooseRandomCoin() {
        float randomChance = Random.value;
        bool defaultChance = true;
        int coinValue = 0;

        // Check for chance and if true, set corresponding sprite and coin value
        for (int i = 0; i < sprites.Length - 1; i++) {
            if (chances[i] > randomChance) {
                spriteRenderer.sprite = sprites[i];
                coinValue = coinValues[i];
                defaultChance = false;
                break;
            }
        }

        // If all chances failed, set sprite and coin value to default, aka last one
        if (defaultChance) {
            spriteRenderer.sprite = sprites[^1];
            coinValue = coinValues[^1];
        }

        GameManager.Instance.AddCoins(coinValue);
    }

    private IEnumerator Animate() {
        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 2f;

        // Moves coin slighlty up and then it returns the coin to original location
        yield return Move(restingPosition, animatedPosition);
        yield return Move(animatedPosition, restingPosition);

        gameObject.SetActive(false);
    }

    private IEnumerator Move(Vector3 from, Vector3 to) {
        float elapsed = 0f;
        float duration = 0.25f;

        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }

}
