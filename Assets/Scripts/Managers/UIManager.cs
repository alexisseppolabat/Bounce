using System.Collections.Generic;
using Structural;
using UnityEngine;
using UnityEngine.UI;

namespace Managers {
    public class UIManager : MonoBehaviour {
        private readonly List<RawImage> rings = new List<RawImage>();
        private readonly Vector2 initRingPos = new Vector2(136, -38);

        public Text livesText;
        public Text scoreText;
        public Scrollbar progressBar;
        public RawImage ring;
        public Canvas canvas;
        public Animator gameOverAnimator;
        public Animator winningAnimator;
        public GameObject gameOverPanel;
        public GameObject winningPanel;
        public GameObject waterEffect;
        public Material obtainedRingMaterial;
        public WorldManager worldManager;

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
            ringCollider.transform.parent.GetComponent<Renderer>().material = obtainedRingMaterial;
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
            // Find how many ingame rings there are
            int numRings = GameObject.FindGameObjectsWithTag("Actual Ring").Length;

            // Place a ring image on the canvas for each ring in the game
            for (int index = 0; index < numRings; index ++) {
                // Create a logical representation of each ring and attach the ring image to the canvas
                RawImage newestRing = Instantiate(ring, canvas.transform, true);
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


        public void UpdateLives() {
            livesText.text = Player.lives.ToString();
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
    }
}