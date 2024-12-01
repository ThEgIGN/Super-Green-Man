using System;
using UnityEngine;
using UnityEngine.UIElements;

public class NextLevel : MonoBehaviour {

    private VisualElement scoreScreen;
    private Label nextLevelLabel, scoreScreenCoins, scoreScreenEnemies, scoreScreenTime, scoreScreenFinalScore;
    private int currentLevel;
    private bool showScore;

    private void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        nextLevelLabel = root.Q("NextLevelLabel") as Label;
        scoreScreen = root.Q("ScoreElement") as VisualElement;
        scoreScreenCoins = scoreScreen.Q("CoinsCollected") as Label;
        scoreScreenEnemies = scoreScreen.Q("EnemiesKilled") as Label;
        scoreScreenTime = scoreScreen.Q("TimeRemaining") as Label;
        scoreScreenFinalScore = scoreScreen.Q("FinalScore") as Label;
    }

    private void Start() {
        ShowNextLevelScreen();
        if (showScore) {
            AudioManager.Instance.PlayMusic("MainMenu");
        } else {
            AudioManager.Instance.PlaySFX("LevelLoading", 0.25f);
        }
    }

    private void Update() {
        nextLevelLabel.text = "Level " + currentLevel +
            " starts in " + GameManager.Instance.loadTimerSecondsRemaining;
    }

    // Only show score if Player finished level, else just show loading screen with loading label
    private void ShowNextLevelScreen() {
        currentLevel = GameManager.Instance.level;
        showScore = GameManager.Instance.displayScore;
        scoreScreen.visible = showScore;

        if (showScore) {
            int coins = GameManager.Instance.coins;
            int enemiesKilled = GameManager.Instance.enemiesKilled;
            int secondsRemaining = Convert.ToInt32(Math.Ceiling(GameManager.Instance.levelTimer));
            scoreScreenCoins.text = "Coins collected: " + coins;
            scoreScreenEnemies.text = "Enemies killed: " + enemiesKilled;
            scoreScreenTime.text = "Time remaining: " + secondsRemaining;
            int finalScore = (coins + (enemiesKilled * 10)) * secondsRemaining;
            scoreScreenFinalScore.text = "Score: " + finalScore;
        }
    }

}
