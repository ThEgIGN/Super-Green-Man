using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockHit : MonoBehaviour {

    public GameObject star, health, life, coinBlock;
    public GameObject enemyToSpawn, goldCoin;
    public Sprite emptyBlock;
    
    // -1 means block can be hit over and over
    public int maxHits = -1;

    public float coinChance = 0.5f;
    public float positivePowerChance = 0.7f;

    // Chances of positive power-ups
    public float extraLifeChance = 0.05f;
    public float starChance = 0.1f;
    public float healthChance = 0.15f;
    public float killAllVisibleEnemiesChance = 0.3f;
    public float spawnCoinsAroundPlayerChance = 0.5f;
    // 2x Coins Power Up is going to be default, or 50% chance

    // Chances of negative power-ups
    public float loseHealthChance = 0.3f;
    public float loseCoinsChance = 0.6f;
    public float spawnEnemiesAround = 0.9f;
    // Lose 1 life is going to be default, aka 10% chance

    private Player player;
    private SpriteRenderer spriteRenderer;

    private bool animating;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animating = false;
    }
    
    private void OnCollisionEnter2D(Collision2D collision) {
        // Player shouldn't be able to hit block again while it's being animated
        if (!animating && collision.gameObject.CompareTag("Player")) {
            player = collision.gameObject.GetComponent<Player>();
            // Player hit the block from below
            if (collision.transform.DotTest(transform, Vector2.up)) {
                Hit();
            }
        }
    }

    private void Hit() {
        AudioManager.Instance.PlaySFX("BlockHit");
        maxHits --;

        // Reveals block in case it was hidden
        spriteRenderer.enabled = true;

        if (maxHits >= 0) {
            // If star object isn't set, Player is hitting the Coin Box block
            if (star == null) {
                Instantiate(coinBlock, transform.position, Quaternion.identity);
            // Player is hitting the Power Up block, so we have to call random logic
            } else {
                float randomChance = Random.value;
                // Player can get coin from Power Up block with certain chance (default 50%)
                if (coinChance > randomChance) {
                    Instantiate(coinBlock, transform.position, Quaternion.identity);
                // For the rest, random Power Up will be chosen
                } else {
                    ChoosePowerRandomly();
                }
            }
            // Render corresponding empty block after Player hit Power Up/Coin block
            if (maxHits == 0) {
                spriteRenderer.sprite = emptyBlock;
            } 
        }

        StartCoroutine(Animate());
    }

    private void ChoosePowerRandomly() {
        float positiveOrNegative = Random.value;
        float choosePowerUp = Random.value;
        // Positive
        if (positivePowerChance > positiveOrNegative) {
            AudioManager.Instance.PlaySFX("PositivePowerUp", 1f);
            if (extraLifeChance > choosePowerUp) {
                Instantiate(life, transform.position, Quaternion.identity);
            } else if (starChance > choosePowerUp) {
                Instantiate(star, transform.position, Quaternion.identity);
            } else if (healthChance > choosePowerUp) {
                Instantiate(health, transform.position, Quaternion.identity);
            } else if (killAllVisibleEnemiesChance > choosePowerUp) {
                // It makes sense only to kill enemies of there's at least 1 of them on the screen
                if (GameManager.Instance.NumberOfVisibleEnemies() > 0) {
                    GameManager.Instance.KillAllVisibleEnemies();
                } else {
                    SpawnCoinsOrEnemiesAround(true);
                }
            } else if (spawnCoinsAroundPlayerChance > choosePowerUp) {
                SpawnCoinsOrEnemiesAround(true);
            } else {
                GameManager.Instance.ActivateDoubleCoins();
            }
        // Negative
        } else {
            AudioManager.Instance.PlaySFX("NegativePowerUp", 0.9f);
            if (loseHealthChance > choosePowerUp) {
                player.Hit(20);
            } else if (loseCoinsChance > choosePowerUp) {
                GameManager.Instance.AddCoins(-10);
            } else if (spawnEnemiesAround > choosePowerUp) {
                SpawnCoinsOrEnemiesAround(false);
            } else {
                GameManager.Instance.AddLife(-1);
            }
        }
    }

    private void DestroyCoinsAfterPeriod(List<GameObject> coins) {
        StartCoroutine(DestroySpawnedCoins(coins));
    }

    private IEnumerator DestroySpawnedCoins(List<GameObject> coins) {
        yield return new WaitForSeconds(7f);
        foreach (GameObject coin in coins) {
            coin.SetActive(false);
        }
    }

    private void SpawnCoinsOrEnemiesAround(bool coins) {
        Vector3 blockPos = gameObject.transform.position;
        float blockX = blockPos.x;
        float blockY = blockPos.y;
        float blockZ = blockPos.z;

        List<GameObject> coinObjects = new List<GameObject>();

        if (coins) {
            // Spawn coins around Box
            for (int x = 2; x > -3; x--) {
                for (int y = 2; y > -3; y--) {
                    // Don't spawn coin behind original Box
                    if (x == 0 && y == 0) {
                        continue;
                    }
                    GameObject coin =
                        Instantiate(goldCoin, new Vector3(blockX + x, blockY + y, blockZ), Quaternion.identity);
                    coinObjects.Add(coin);
                }
            }
            // Destroys coins after 5 seconds
            DestroyCoinsAfterPeriod(coinObjects);
        } else {
            Instantiate(enemyToSpawn, new Vector3(blockX + 8f, blockY + 4f, blockZ), Quaternion.identity);
            Instantiate(enemyToSpawn, new Vector3(blockX - 8f, blockY + 4f, blockZ), Quaternion.identity);
        }
    }

    private IEnumerator Animate() {
        animating = true;

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 0.35f;

        // Moves block slighlty up and then it returns the block to original location
        yield return Move(restingPosition, animatedPosition);
        yield return Move(animatedPosition, restingPosition);

        animating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to) {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration) {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }

}
