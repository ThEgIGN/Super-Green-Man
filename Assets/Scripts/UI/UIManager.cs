using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour {

    public const string Health = "Health";
    public const string Lives = "Lives";
    public const string AddCoins = "AddCoins";
    public const string RemoveCoins = "RemoveCoins";
    public const string Level = "Level";
    public const string LevelTimer = "LevelTimer";
    public const string DoubleCoinsOn = "DoubleCoinsOn";
    public const string DoubleCoinsOff = "DoubleCoinsOff";
    public const string GameOver = "GameOver";

    public Sprite fullHeart;

    // Force update to get fresh values
    private string[] updateUIAtLoadValues = {Health, Lives, AddCoins, Level, LevelTimer, DoubleCoinsOff};

    private Label labelCoins, labelLevel, labelLevelTimer, labelDoubleCoins, labelMessage;

    private VisualElement heartContainer;
    private VisualElement gameOverScreen;

    private VisualElement healthBar;
    private Label healthBarLabel;
    private int maxHealth = 0;

    private Button newGameButton;
    private Button mainMenuButton;
    private List<Button> allButtons;

    private bool messageShown;
    private float messageShownTimer;

    public static UIManager Instance;
    
    private void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } else { 
            Instance = this;
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            labelLevelTimer = root.Q("Timer") as Label;
            labelCoins = root.Q("Coins") as Label;
            labelLevel = root.Q("Level") as Label;
            labelDoubleCoins = root.Q("DoubleCoins") as Label;
            labelMessage = root.Q("Message") as Label;
            
            heartContainer = root.Q("LivesContainer") as VisualElement;
            VisualElement health = root.Q("HealthBarBackground") as VisualElement;
            healthBar = health.Q("HealthBarOverlay") as VisualElement;
            healthBarLabel = health.Q("HealthBarText") as Label;

            gameOverScreen = root.Q("GameOver") as VisualElement;
            newGameButton = gameOverScreen.Q("NewGameButton") as Button;
            newGameButton.RegisterCallback<ClickEvent>(NewGameClick);
            mainMenuButton = gameOverScreen.Q("MainMenuButton") as Button;
            mainMenuButton.RegisterCallback<ClickEvent>(MainMenuClick);

            allButtons = root.Query<Button>().ToList();
            foreach (Button button in allButtons) {
                button.RegisterCallback<ClickEvent>(PlaySoundOnClick);
            }

            messageShownTimer = 0f;
            messageShown = false;
        }
    }

    private void Start() {
        EnableGameOverScreen(false);
        UpdateAll();
    }

    private void OnDisable() {
        newGameButton.UnregisterCallback<ClickEvent>(NewGameClick);
        mainMenuButton.UnregisterCallback<ClickEvent>(MainMenuClick);
        foreach (Button button in allButtons) {
            button.UnregisterCallback<ClickEvent>(PlaySoundOnClick);
        }
    }

    private void NewGameClick(ClickEvent clickEvent) {
        // At first this would start New Game, aka Level 1, but after some thinking,
        // I thought it would be better to make this button reload current level
        int currentLevel = GameManager.Instance.level;
        GameManager.Instance.LoadLevelAfterDelay(3f, currentLevel, true, true, false);
    }

    private void MainMenuClick(ClickEvent clickEvent) {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void PlaySoundOnClick(ClickEvent clickEvent) {
        // SoundManager.Instance.PlayButtonSound
    }

    private void UpdateAll() {
        foreach (string component in updateUIAtLoadValues) {
            UpdateSpecificUIComponent(component);
        }
    }

    private void EnableGameOverScreen(bool enable) {
        gameOverScreen.visible = enable;
    }

    // Make hearts visible/invisible based on amount of lives
    private void UpdateHearts() {
        int lives = GameManager.Instance.lives;
        VisualElement[] heartsArray = heartContainer.Children().ToArray();
        for (int i = 0; i < lives; i++) {
            heartsArray[i].visible = true;
        }
        for (int i = lives; i < 5; i++) {
            heartsArray[i].visible = false;
        }
    }

    // Adjust health bar according to Players max health and current health
    private void UpdateHealth() {
        int currentHealth = GameManager.Instance.health;
        if (currentHealth > maxHealth) {
            maxHealth = currentHealth;
        }
        healthBarLabel.text = "Health: " + currentHealth + "/" + maxHealth;
        healthBar.style.width = Length.Percent((100 * currentHealth) / maxHealth);
    }

    public void UpdateSpecificUIComponent(string component) {
        switch(component) {
            case Health:
                UpdateHealth();
                break;
            case Lives:
                UpdateHearts();
                break;
            case AddCoins:
                labelCoins.text = GameManager.Instance.coins.ToString();
                break;
            // When Player loses health or life from negative Power Up, they can see it clearly
            // (Player gets hit animation or heart dissapears from top of screen)
            // However, with coins, it felt like Player couldn't see it clearly, so message is shown for that
            case RemoveCoins:
                labelCoins.text = GameManager.Instance.coins.ToString();
                labelMessage.text = "If you don't mind, I'll be taking those coins :)";
                DeleteMessageAfterTime(3f);
                break;
            case Level:
                labelLevel.text = "Level " + GameManager.Instance.level;
                break;
            case LevelTimer:
                labelLevelTimer.text = GameManager.Instance.levelTimerString;
                break;
            case DoubleCoinsOn:
                labelDoubleCoins.text =
                    "Time remaining on 2x Coins: " + GameManager.Instance.doubleCoinsSecondsRemaining;
                break;
            case DoubleCoinsOff:
                labelDoubleCoins.text = "";
                break;
            case GameOver:
                EnableGameOverScreen(true);
                break;
        }
    }

    private void DeleteMessageAfterTime(float time) {
        StartCoroutine(DeleteMessage(time));
    }

    private IEnumerator DeleteMessage(float time) {
        // If there is already active message, just change timer
        if (messageShown) {
            messageShownTimer = time;
            yield return null;
        } else {
            messageShown = true;
            messageShownTimer = time;

            while (messageShownTimer > 0f) {
                messageShownTimer -= Time.deltaTime;
                yield return null;
            }

            messageShownTimer = 0f;
            messageShown = false;
            labelMessage.text = "";
        }
    }

}
