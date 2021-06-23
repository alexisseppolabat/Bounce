using UnityEngine;
using UnityEngine.UI;

namespace Managers {
    public class MenuManager : MonoBehaviour {
        private bool limitedLives = true;

        public GameObject menuCanvas;
        public GameObject levelSelectorCanvas;
        public GameObject optionsCanvas;
        public Text livesText;

        public Slider musicSlider;
        public Slider effectsSlider;
        
        public void ClickPlay() {
            menuCanvas.SetActive(false);
            levelSelectorCanvas.SetActive(true);
        }

        public void ClickOptions() {
            menuCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
        }

        public void ClickExit() {
            
        }

        public void LevelSelectorClickBack() {
            menuCanvas.SetActive(true);
            levelSelectorCanvas.SetActive(false);
        }

        public void SelectLevel(int level) {
            Debug.Log(level);
        }

        public void OptionsClickBack() {
            menuCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
        }

        public void ToggleLives() {
            limitedLives = !limitedLives;
            livesText.text = limitedLives ? "limited" : "unlimited";
        }
    }
}
