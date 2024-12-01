using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour {

    // Idea is to have one GamaManager for all levels, so when Player switches to next level,
    // previous GamaManager isn't destroyed and same is being used
    public static GameManager Instance { get; private set; }

    public int level { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; }
    public int health { get; private set; }
    public int enemiesKilled { get; private set; }

    // Level and coin timer should only run while Player is playing level
    // When Player finishes level and touches Exit door, both timers should stop
    private bool levelBeingPlayed = false;

    public bool LevelBeingPlayed {
        get { return levelBeingPlayed; }
        set { levelBeingPlayed = value; }
    }

    // Publicly available Double Coin and its timer
    private float doubleCoinsTimer;
    public bool doubleCoins { get; private set; }
    public int doubleCoinsSecondsRemaining { get; private set; }

    // Each level should be completed in under 300 seconds
    public float levelTimerDuration = 300f;
    public float levelTimer { get; private set; }
    public string levelTimerString;

    public int loadTimerSecondsRemaining { get; private set; }
    public bool displayScore { get; private set; }

    // Private list of visible enemies on screen
    private List<GameObject> enemiesOnScreen;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    // Countdown for level timer
    private void Update() {
        if (levelBeingPlayed) {
            if (levelTimer > 0) {
                levelTimer -= Time.deltaTime;
                UpdateLevelTimer(levelTimer);
            } else {
                levelBeingPlayed = false;
                ResetLevel();
            }
        }
    }

    private void UpdateLevelTimer(float time) {
        int minutes = Convert.ToInt32(Math.Ceiling(time)) / 60;
        int seconds = Convert.ToInt32(Math.Ceiling(time)) % 60;

        // Formats timer in 00:00 format
        levelTimerString = string.Format("{00:00}:{1:00}", minutes, seconds);
        UIManager.Instance.UpdateSpecificUIComponent(UIManager.LevelTimer);
    }

    private void Start() {
        Application.targetFrameRate = 60;

        // // Delete this whole thing after testing
        // this.level = 3;
        // health = 100;
        // enemiesKilled = 0;
        // doubleCoinsTimer = 0f;
        // doubleCoins = false;
        // levelTimer = levelTimerDuration;
        // levelBeingPlayed = true;
        // coins = 0;
        // lives = 3;
        // enemiesOnScreen = new List<GameObject>();
        // // Delete this whole thing after testing
    }

    public void LoadLevelAfterDelay(float delay, int level, bool resetCoins, bool resetLives, bool score) {
        this.level = level;
        health = 100;
        StartCoroutine(LoadLevel(delay, level, resetCoins, resetLives, score));
    }

    private IEnumerator LoadLevel(float delay, int level, bool resetCoins, bool resetLives, bool score) {
        displayScore = score;
        SceneManager.LoadScene("NextLevelScene");

        float loadTimer = delay;

        while (loadTimer > 0f) {
            loadTimer -= Time.deltaTime;
            loadTimerSecondsRemaining = Convert.ToInt32(Math.Ceiling(loadTimer));
            yield return null;
        }

        LoadLevel(level, resetCoins, resetLives);
    }

    // Everytime level gets loaded, set coresponding level and reset coins, multiplier, timer
    private void LoadLevel(int level, bool resetCoins, bool resetLives) {
        // If Player finished all 3 levels, load Victory scene
        if (level > 3) {
            SceneManager.LoadScene("VictoryScene");
        } else {
            this.level = level;
            health = 100;
            enemiesKilled = 0;
            doubleCoinsTimer = 0f;
            doubleCoins = false;
            levelTimer = levelTimerDuration;
            levelBeingPlayed = true;
            enemiesOnScreen = new List<GameObject>();

            // If Player started level for the first time, coins should be reset
            if (resetCoins) {
                coins = 0;
            }

            // Lives are only reset when Player starts playing game for first time (New Game button is pressed)
            if (resetLives) {
                lives = 3;
            }

            string playLevel = "Level" + level;
            SceneManager.LoadScene(playLevel);
            AudioManager.Instance.PlayMusic(playLevel, 0.25f);
        }
    }

    // Calls function after delay
    private void ResetLevel(float delay) {
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel() {
        lives--;

        // When Player respawns, don't show "Level starts in..." screen, just Load scene immediately
        if (lives > 0) {
            LoadLevel(level, false, false);
        } else {
            GameOver();
        }
    }

    private void GameOver() {
        AudioManager.Instance.musicSource.Stop();
        AudioManager.Instance.PlaySFX("GameOver");
        UIManager.Instance.UpdateSpecificUIComponent(UIManager.GameOver);
    }

    public void AddCoins(int coins) {
        int newCoins = doubleCoins ? 2 * coins : coins;
        // Player can't have negative coins
        this.coins = Math.Max(this.coins + newCoins, 0);
        if (coins < 0) {
            UIManager.Instance.UpdateSpecificUIComponent(UIManager.RemoveCoins);
        } else {
            AudioManager.Instance.PlaySFX("Coin1", 0.75f);
            UIManager.Instance.UpdateSpecificUIComponent(UIManager.AddCoins);
        }
    }

    public void AddLife(int newLife) {
        // Player can have maximum of 5 lives
        // If negative PowerUp tries taking Players last life, nothing happens
        lives = Math.Clamp(lives + newLife, 1, 5);
        UIManager.Instance.UpdateSpecificUIComponent(UIManager.Lives);
    }

    public void EnemyKilled() {
        AudioManager.Instance.PlaySFX("EnemyDeath", 0.6f);
        enemiesKilled++;
    }

    public void ChangeHealth(int newHealth) {
        health = newHealth;
        UIManager.Instance.UpdateSpecificUIComponent(UIManager.Health);
        if (health == 0) {
            levelBeingPlayed = false;
            // Death animation lasts 3f, so we restart level after same duration
            ResetLevel(3f);
        }
    }

    public void ActivateDoubleCoins() {
        StartCoroutine(DoubleCoinsTimer(10f));
    }

    private IEnumerator DoubleCoinsTimer(float time) {
        // If there is already active multiplier going, add time to it and do nothing else
        if (doubleCoins) {
            doubleCoinsTimer += time;
            yield return null;
        } else {
            doubleCoins = true;
            doubleCoinsTimer += time;

            while (levelBeingPlayed && doubleCoins && doubleCoinsTimer > 0f) {
                doubleCoins = true;
                doubleCoinsTimer -= Time.deltaTime;
                doubleCoinsSecondsRemaining = Convert.ToInt32(Math.Ceiling(doubleCoinsTimer));
                UIManager.Instance.UpdateSpecificUIComponent(UIManager.DoubleCoinsOn);
                yield return null;
            }

            doubleCoinsTimer = 0f;
            doubleCoins = false;
            UIManager.Instance.UpdateSpecificUIComponent(UIManager.DoubleCoinsOff);
        }
    }

    public void FreezeAllVisibleEnemies() {
        // Only freeze enemies that implement interface Freezable
        foreach (GameObject enemy in enemiesOnScreen) {
            if (enemy.TryGetComponent<Freezable>(out Freezable freezable)) {
                freezable.Freeze();
            }
        }
    }

    public void KillAllVisibleEnemies() {
        foreach (GameObject enemy in enemiesOnScreen) {
            // Only destroys game objects that are Hittable
            if (enemy.TryGetComponent<Hittable>(out Hittable hittable)) {
                hittable.Hit(1000);
            }
        }
    }

    public void KillAllVisibleEnemiesInRadius(GameObject caller, Vector3 position, float radius) {
        AudioManager.Instance.PlaySFX("Explosion", 0.45f);
        foreach (GameObject enemy in enemiesOnScreen) {
            // Don't hit caller of the function. Bomb shouldn't kill itself
            if (caller != enemy) {
                // Only destroys game objects that are Hittable and close to gameObject (bomb)
                float distance = Vector3.Distance(enemy.transform.position, position);
                if (distance <= radius && enemy.TryGetComponent<Hittable>(out Hittable hittable)) {
                    hittable.Hit(1000);
                }
            }
        }
    }

    public int NumberOfVisibleEnemies() {
        return enemiesOnScreen.Count;
    }

    public void RemoveInvisibleEnemyFromList(GameObject gameObject) {
        int index = enemiesOnScreen.IndexOf(gameObject);
        if (index < 0) {
            return;
        }
        enemiesOnScreen.RemoveAt(index);
    }
 
    public void AddVisibleEnemyToList(GameObject gameObject) {
        if (enemiesOnScreen.Contains(gameObject)) {
            return;
        }
        enemiesOnScreen.Add(gameObject);
    }

}
