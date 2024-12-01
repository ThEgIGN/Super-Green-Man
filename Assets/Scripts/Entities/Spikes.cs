using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour {

    public float timerDuration = 5f;
    private float timer;
    private bool inBlock;
    public int spikesDamage = 100;
    // left, right, up, down
    public string direction;
    public bool startActive;

    private Vector3 restingPosition;
    private Vector3 activePosition;

    private void Awake() {
        inBlock = true;
        // Whether spikes should start appearing immediately or after timerDuration
        timer = startActive ? -1f : timerDuration;
        ConfigureDirection();
    }

    private void ConfigureDirection() {
        Vector3 activeDirection;

        switch (direction) {
            case "down":
                // Flips spikes sprite
                transform.eulerAngles = new Vector3(180f, 0f, 0f);
                // Change direction of spikes
                activeDirection = Vector3.down;
                break;
            case "left":
                transform.eulerAngles = new Vector3(0f, 0f, 90f);
                activeDirection = Vector3.left;
                break;
            case "right":
                transform.eulerAngles = new Vector3(0f, 0f, 270f);
                activeDirection = Vector3.right;
                break;
            default:
                activeDirection = Vector3.up;
                break;
        }

        restingPosition = transform.localPosition;
        activePosition = restingPosition + activeDirection;
    }

    // Spikes won't be disabled on becoming invisible, since there might be parts of level
    // that require specific timing that shouldn't be changed with enabling/disabling

    private void Update() {
        timer -= Time.deltaTime;
        if (timer < 0) {
            timer = timerDuration;
            StartCoroutine(Animate());
            inBlock = !inBlock;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Spikes should only damage Player
        if (collision.gameObject.CompareTag("Player")) {
            Player player = collision.gameObject.GetComponent<Player>();
            player.Hit(spikesDamage);
        }
    }

    private IEnumerator Animate() {
        // Moves spikes up or down based on position
        if (inBlock) {
            yield return Move(restingPosition, activePosition);
        } else {
            yield return Move(activePosition, restingPosition);
        }
    }

    private IEnumerator Move(Vector3 from, Vector3 to) {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }

}
