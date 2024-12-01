using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour {

    private Button buttonLevel1, buttonLevel2, buttonLevel3, buttonQuit;
    private List<Button> allButtons;

    private void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        buttonLevel1 = root.Q("ButtonLevel1") as Button;
        buttonLevel1.RegisterCallback<ClickEvent>(PlayLevel1);
        buttonLevel2 = root.Q("ButtonLevel2") as Button;
        buttonLevel2.RegisterCallback<ClickEvent>(PlayLevel2);
        buttonLevel3 = root.Q("ButtonLevel3") as Button;
        buttonLevel3.RegisterCallback<ClickEvent>(PlayLevel3);
        buttonQuit = root.Q("ButtonQuit") as Button;
        buttonQuit.RegisterCallback<ClickEvent>(QuitGame);

        allButtons = root.Query<Button>().ToList();
        foreach (Button button in allButtons) {
            button.RegisterCallback<ClickEvent>(PlaySoundOnClick);
        }
    }

    // When game starts, music will be played
    private void Start() {
        AudioManager.Instance.PlayMusic("MainMenu");
    }

    // Every time Player returns to Main Menu, play music
    private void OnEnable() {
        AudioManager.Instance.PlayMusic("MainMenu");
    }

    private void OnDisable() {
        buttonLevel1.UnregisterCallback<ClickEvent>(PlayLevel1);
        buttonLevel2.UnregisterCallback<ClickEvent>(PlayLevel2);
        buttonLevel3.UnregisterCallback<ClickEvent>(PlayLevel3);
        buttonQuit.UnregisterCallback<ClickEvent>(QuitGame);
        foreach (Button button in allButtons) {
            button.UnregisterCallback<ClickEvent>(PlaySoundOnClick);
        }
    }

    private void PlayLevel1(ClickEvent clickEvent) {
        GameManager.Instance.LoadLevelAfterDelay(3f, 1, true, true, false);
    }

    private void PlayLevel2(ClickEvent clickEvent) {
        GameManager.Instance.LoadLevelAfterDelay(3f, 2, true, true, false);
    }

    private void PlayLevel3(ClickEvent clickEvent) {
        GameManager.Instance.LoadLevelAfterDelay(3f, 3, true, true, false);
    }

    private void QuitGame(ClickEvent clickEvent) {
        Application.Quit();
    }

    private void PlaySoundOnClick(ClickEvent clickEvent) {
        AudioManager.Instance.musicSource.Stop();
        AudioManager.Instance.PlaySFX("Button", 1f);
    }

}
