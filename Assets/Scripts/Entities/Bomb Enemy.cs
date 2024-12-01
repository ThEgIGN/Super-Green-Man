using UnityEngine;

public class BombEnemy : MonoBehaviour, Hittable, Freezable {

    public int bombEnemyDamage = 20;
    public float bombRadius = 7f;

    private DeathAnimation deathAnimation;
    private EntityMovement entityMovement;
    private SpriteRenderer spriteRenderer;
    private new Rigidbody2D rigidbody2D;
    private Player player;
    private GameObject playerObject;

    public Sprite bomb;

    public AnimatedSprite enemyWalking;
    public AnimatedSprite bombLightingFuse;
    public AnimatedSprite bombExploding;

    private bool bombActive = false;
    private bool shouldBombDamageEntities = true;

    private void Awake() {
        entityMovement = GetComponent<EntityMovement>();
        deathAnimation = GetComponent<DeathAnimation>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerObject = GameObject.FindWithTag("Player");
        player = playerObject.GetComponent<Player>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Player shouldn't be able to damage or be damaged by bomb
        if (!bombActive && collision.gameObject == playerObject) {
            if (player.starPower) {
                Hit(1000);
            } else if (collision.transform.DotTest(transform, Vector2.down)) {
                Hit();
            } else {
                player.Hit(bombEnemyDamage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (!bombActive && collider.gameObject.layer == LayerMask.NameToLayer("Shell")) {
            Hit(1000);
        }
    }

    public void Hit(int damage = 100) {
        // Star power or death barrier shouldn't activate bomb
        if (damage == 100000) {
            Death();
        } else if (damage == 1000) {
            GameManager.Instance.EnemyKilled();
            Death();
        } else {
            GameManager.Instance.EnemyKilled();
            SwitchToBomb();
        }
    }

    private void SwitchToBomb() {
        bombActive = true;

        entityMovement.enabled = false;
        enemyWalking.enabled = false;
        spriteRenderer.sprite = bomb;
        spriteRenderer.flipX = false;
        // Player shouldn't be able to push bomb around
        rigidbody2D.isKinematic = true;

        bombLightingFuse.enabled = true;
        BlowUpAfterTime(2.5f);
    }

    private void BlowUpAfterTime(float time) {
        Invoke(nameof(BlowUp), time);
    }

    private void BlowUp() {
        bombLightingFuse.enabled = false;
        bombExploding.enabled = true;
        TryToKillEverybodyAfterTime(0.2f);
        DisableEnemyAfterPeriod(1f);
    }

    private void TryToKillEverybodyAfterTime(float time) {
        Invoke(nameof(TryToKillEverybody), time);
    }

    private void TryToKillEverybody() {
        if (shouldBombDamageEntities) {
            float distance = Vector3.Distance(playerObject.transform.position, transform.position);
            if (distance <= bombRadius) {
                player.Hit(1000);
            }
            GameManager.Instance.KillAllVisibleEnemiesInRadius(gameObject, transform.position, bombRadius);
        }
    }

    public void Freeze() {
        if (bombActive) {
            shouldBombDamageEntities = false;
        } else {
            entityMovement.enabled = false;
        }
    }

    private void Death() {
        enemyWalking.enabled = false;
        deathAnimation.enabled = true;
        DisableEnemyAfterPeriod(3f);
    }

    private void OnBecameVisible() {
        GameManager instance= GameManager.Instance;
        // Bomb shouldn't be freezable or hittable by GameManager
        if (!bombActive && instance != null) {
            instance.AddVisibleEnemyToList(gameObject);
        }
    }

    private void OnBecameInvisible() {
        GameManager instance= GameManager.Instance;
        if (!bombActive && instance != null) {
            instance.RemoveInvisibleEnemyFromList(gameObject);
        }

        // Disable bomb if its off screen
        if (bombActive) {
            DisableEnemy();
        }
    }
    
    private void DisableEnemy() {
        gameObject.SetActive(false);
    }

    private void DisableEnemyAfterPeriod(float time) {
        Invoke(nameof(DisableEnemy), time);
    }

}
