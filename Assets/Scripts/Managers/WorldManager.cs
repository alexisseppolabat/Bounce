using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers {
    public class WorldManager : MonoBehaviour {

        public Animator openGate;
        public UIManager uiManager;

        private void Start() {
            // Initialise all of the player values before initialising the UI manager
            Structural.Player.Init();
            uiManager.Init();
        }


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