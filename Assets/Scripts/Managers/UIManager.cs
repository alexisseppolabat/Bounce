using System;
using System.Collections.Generic;
using Structural;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers {
    public class UIManager : MonoBehaviour {
        private readonly List<RawImage> rings = new List<RawImage>();
        private readonly Vector2 initRingPos = new Vector2(136, -38);
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        public Text livesText;
        public Text scoreText;
        public Scrollbar progressBar;
        public RawImage ring;
        public Animator gameOverAnimator;
        public Animator winningAnimator;
        public GameObject gameOverPanel;
        public GameObject winningPanel;
        public GameObject waterEffect;
        public WorldManager worldManager;

        public GameObject menu;
        public GameObject menuPanel;
        public GameObject optionsPanel;
        public Slider musicSlider;
        public Slider effectsSlider;
        
        public void Start() {
            UpdateScore();
            UpdateLives();
            DrawRings();
        }


        public void PlayGameOverAnimation() {
            gameOverPanel.SetActive(true);
            gameOverAnimator.enabled = true;
        }

        public void PlayWinningAnimation() {
            winningPanel.SetActive(true);
            winningAnimator.enabled = true;
        }

        public void ActivateWaterEffect() {
            waterEffect.SetActive(true);
        }

        public void DeactivateWaterEffect() {
            waterEffect.SetActive(false);
        }

        public void ObtainRing(Collider ringCollider) {
            // Change the ring's colour from gold to white to indicate that it has been obtained
            ringCollider.transform.parent.GetComponent<Renderer>().material.SetColor(ColorID, Color.white);
            // And disable the ring trigger so that it doesn't get triggered again
            ringCollider.enabled = false;

            // Remove one ring from the canvas UI
            RawImage lastRing = rings[rings.Count - 1];
            lastRing.enabled = false;
            rings.Remove(lastRing);

            // Open the end gate once the last ring has been gotten
            if (rings.Count == 0) worldManager.OpenEndGate();
        }

        private void DrawRings() {
            // Find how many in game rings there are
            int numRings = GameObject.FindGameObjectsWithTag("Actual Ring").Length;

            // Place a ring image on the canvas for each ring in the game
            for (int index = 0; index < numRings; index ++) {
                // Create a logical representation of each ring and attach the ring image to the canvas
                RawImage newestRing = Instantiate(ring, transform, true);
                rings.Add(newestRing);

                // Attach the ring image to an anchor situated at the top left of the canvas
                newestRing.rectTransform.anchorMin = new Vector2(0, 1);
                newestRing.rectTransform.anchorMax = new Vector2(0, 1);
                newestRing.rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // Place the ring image in the appropriate position and ensure its size is correct
                newestRing.rectTransform.anchoredPosition = new Vector2(10 * index, 0) + initRingPos;
                newestRing.rectTransform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Time.timeScale = 0;
                menu.SetActive(true);
            }
        }


        public void UpdateLives() {
            livesText.text = Player.limitedLives ? Player.lives.ToString() : "∞";
        }

        public void UpdateScore() {
            scoreText.text = Player.score.ToString();
        }

        public void ResetProgressBar() {
            progressBar.value = 0f;
        }

        public void SetProgressBar(float value) {
            progressBar.value = value;
        }

        public void ClickResume() {
            Time.timeScale = 1;
            menu.SetActive(false);
        }

        public void ClickOptions() {
            optionsPanel.SetActive(true);
            menuPanel.SetActive(false);
        }

        public void ClickMenu() {
            Time.timeScale = 1;
            SceneManager.LoadScene("Scenes/Menu");
        }

        public void ClickBack() {
            // Update the music and effects volumes
            MenuManager.musicVolume = musicSlider.value;
            MenuManager.effectsVolume = effectsSlider.value;
            
            // And go back to the menu panel
            optionsPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }
}