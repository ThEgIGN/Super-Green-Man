using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class VictoryScene : MonoBehaviour {

    private Button menuButton;

    private void Awake() {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        menuButton = root.Q("MenuButton") as Button;
        menuButton.RegisterCallback<ClickEvent>(MainMenuClick);
        menuButton.RegisterCallback<ClickEvent>(PlaySoundOnClick);
    }

    private void OnEnable() {
        AudioManager.Instance.PlayMusic("Victory");
    }

    private void OnDisable() {
        menuButton.UnregisterCallback<ClickEvent>(MainMenuClick);
        menuButton.UnregisterCallback<ClickEvent>(PlaySoundOnClick);
    }

    private void MainMenuClick(ClickEvent clickEvent) {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void PlaySoundOnClick(ClickEvent clickEvent) {
        AudioManager.Instance.PlaySFX("Button", 1f);
    }

}
