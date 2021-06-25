using Structural;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers {
    public class MenuManager : MonoBehaviour {
        public GameObject menuCanvas;
        public GameObject levelSelectorCanvas;
        public GameObject optionsCanvas;
        public Text livesText;
        public Slider musicSlider;
        public Slider effectsSlider;

        public static float musicVolume;
        public static float effectsVolume;
        
        public void ClickPlay() {
            menuCanvas.SetActive(false);
            levelSelectorCanvas.SetActive(true);
        }

        public void ClickOptions() {
            menuCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
        }

        public void ClickExit() {
            Application.Quit();
        }

        public void LevelSelectorClickBack() {
            menuCanvas.SetActive(true);
            levelSelectorCanvas.SetActive(false);
        }

        public void SelectLevel(int level) {
            SceneManager.LoadScene("Scenes/Levels/Level" + level);
        }

        public void OptionsClickBack() {
            musicVolume = musicSlider.value;
            effectsVolume = effectsSlider.value;
            
            menuCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
        }

        public void ToggleLives() {
            Player.limitedLives = !Player.limitedLives;
            livesText.text = Player.limitedLives ? "limited" : "unlimited";
        }
    }
}
