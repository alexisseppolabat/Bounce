using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers {
    public class WorldManager : MonoBehaviour {

        public Animator openGate;

        public void OpenEndGate() {
            openGate.enabled = true;
            openGate.Play("Open Gate");
        }

        public void ReloadLevel() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadLevelSelector() {
            SceneManager.LoadScene("Menu");
        }

        public void LoadNextLevel() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}