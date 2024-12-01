using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, Hittable {

    private DeathAnimation deathAnimation;
    private SpriteRenderer spriteRenderer;
    public PlayerSpriteRenderer playerSpriteRenderer;

    private int health;
    public bool starPower { get; private set; }
    public float invincibilityDuration = 1f;

    private float damageInvincibilty;

    private void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        deathAnimation = GetComponent<DeathAnimation>();
        
        health = 100;
        damageInvincibilty = 0f;
    }

    // Countdown time for invincibility
    private void Update() {
        if (damageInvincibilty > 0) {
            damageInvincibilty -= Time.deltaTime;
        }
    }

    public void Hit(int damage) {
        // Even though Player might be in star mode, they should still die when they fall off the map
        // Thus Death barrier sends insane 100000 damage, so we check for it this way
        if (damage == 100000) {
            health = 0;
            Death();
        }
        // Player doesn't take damage if they are dead or in star power
        if (health > 0 && !starPower) {
            // Player is in invinicible mode, so they shouldn't take damage
            if (damageInvincibilty > 0) {
                return;
            }
            // Player has invincibility when they take damage
            damageInvincibilty = invincibilityDuration;
            // Health should never go below 0 or above 200
            health = Math.Clamp(health - damage, 0, 200);
            if (health == 0) {
                Death();
            } else {
                AudioManager.Instance.PlaySFX("PlayerHit", 1f);
                GameManager.Instance.ChangeHealth(health);
                StartCoroutine(HitAnimation());
            }
        }
    }

    public void setDamageInvincibility(float time) {
        damageInvincibilty = time;
    }

    public void Heal(int heal) {
        health = Math.Clamp(health + heal, 0, 200);
        GameManager.Instance.ChangeHealth(health);
    }

    private void Death() {
        AudioManager.Instance.musicSource.Stop();
        AudioManager.Instance.PlaySFX("PlayerDeath", 0.6f);
        GameManager.Instance.FreezeAllVisibleEnemies();
        playerSpriteRenderer.enabled = false;
        deathAnimation.enabled = true;
        GameManager.Instance.ChangeHealth(health);
    }

    private IEnumerator HitAnimation() {
        float elapsed = 0f;
        float duration = invincibilityDuration;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            
            // Every 4th frame, turn renderer on/off, to get that flickering effect on Player
            if (Time.frameCount % 4 == 0) {
                playerSpriteRenderer.enabled = !playerSpriteRenderer.enabled;
            }

            yield return null;
        }

        playerSpriteRenderer.enabled = true;
    }

    public void StarPower(float duration = 10f) {
        StartCoroutine(StarPowerAnimation(duration));
    }

    private IEnumerator StarPowerAnimation(float duration) {
        float elapsed = 0f;

        while (elapsed < duration) {
            starPower = true;
            elapsed += Time.deltaTime;

            // Every 4th frame, set color of renderer to random one, giving Player that crazy rainbow effect
            if (Time.frameCount % 4 == 0) {
                spriteRenderer.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);
            }

            yield return null;
        }

        // Reset to default color
        spriteRenderer.color = Color.white;
        starPower = false;
    }

}
