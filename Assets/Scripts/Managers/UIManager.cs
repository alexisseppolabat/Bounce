using System.Collections.Generic;
using Structural;
using UnityEngine;
using UnityEngine.UI;

namespace Managers {
    public class UIManager : MonoBehaviour {
        private List<RawImage> rings = new List<RawImage>();
        private Vector2 INIT_RING_POS = new Vector2(136, -38);

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
        public RectTransform canvasRectTransform;
        public Material obtainedRingMaterial;
        public WorldManager WorldManager;

        public void Start() {
            updateScore();
            updateLives();
            drawRings();
        }


        public void playDeathAnimation() {
            gameOverPanel.SetActive(true);
            gameOverAnimator.enabled = true;
            gameOverAnimator.Play("Game Over");
        }

        public void playWinningAnimation() {
            winningPanel.SetActive(true);
            winningAnimator.enabled = true;
            winningAnimator.Play("Winning");
        }

        public void activateWaterEffect() {
            waterEffect.SetActive(true);
        }

        public void deactivateWaterEffect() {
            waterEffect.SetActive(false);
        }

        public void obtainRing(Collider collider) {
            // Change the ring's colour from gold to white to indicate that it has been obtained
            collider.transform.parent.GetComponent<Renderer>().material = obtainedRingMaterial;
            // And disable the ring trigger so that it doesn't get triggered again
            collider.enabled = false;

            // Remove one ring from the canvas UI
            RawImage lastRing = rings[rings.Count - 1];
            lastRing.enabled = false;
            rings.Remove(lastRing);

            // Open the end gate once the last ring has been gotten
            if (rings.Count == 0) WorldManager.openEndGate();
        }

        public void drawRings() {
            // Find how many ingame rings there are
            int numRings = GameObject.FindGameObjectsWithTag("Actual Ring").Length;

            // Place a ring image on the canvas for each ring in the game
            for (int index = 0; index < numRings; index ++) {
                // Create a logical representation of each ring
                RawImage newestRing = Object.Instantiate(ring);
                rings.Add(newestRing);

                // Attach the ring image to the canvas
                newestRing.transform.SetParent(canvas.transform);

                // Attach the ring image to an anchor situated at the top left of the canvas
                newestRing.rectTransform.anchorMin = new Vector2(0, 1);
                newestRing.rectTransform.anchorMax = new Vector2(0, 1);
                newestRing.rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // Place the ring image in the appropriate position and ensure its size is correct
                newestRing.rectTransform.anchoredPosition = new Vector2(10 * index, 0) + INIT_RING_POS;
                newestRing.rectTransform.localScale = new Vector3(1, 1, 1);
            }
        }


        public void updateLives() {
            livesText.text = Player.lives.ToString();
        }

        public void updateScore() {
            scoreText.text = Player.score.ToString();
        }

        public void resetProgressBar() {
            progressBar.value = 0f;
        }

        public void setProgressBar(float value) {
            progressBar.value = value;
        }
    }
}