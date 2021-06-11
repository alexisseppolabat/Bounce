using System.Collections.Generic;
using Structural;
using UnityEngine;
using static Structural.Effects;

namespace Managers  {
    public class PlayerCollisionManager : MonoBehaviour {
        public UIManager uiManager;
        public Rigidbody playerBody;

        private readonly HashSet<Effects> expiredEffects = new HashSet<Effects>();


        private void Start() {
            // Initialise all of the player values
            Player.Init();
        }

        private void OnCollisionEnter(Collision collision) {
            HandleCollider(collision.collider);
        }

        private void OnTriggerEnter(Collider triggerCollider) {
            HandleCollider(triggerCollider);
        }
    
        private void OnTriggerStay(Collider triggerCollider) {
            HandleCollider(triggerCollider);
        }

        private void OnTriggerExit(Collider triggerCollider) {
            if (triggerCollider.CompareTag("Water")) playerBody.drag = 0;
        }

        private void HandleCollider(Collider objCollider) {
            switch (objCollider.tag) {
                case "Deflator":
                    // Only deflate the ball if it is big
                    if (Mathf.RoundToInt(gameObject.transform.localScale.magnitude) == Player.Big)
                        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case "Inflator":
                    // Only inflate the ball if it is small
                    if (Mathf.RoundToInt(gameObject.transform.localScale.magnitude) == Player.Small)
                        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    break;
                case "Pop":
                    Player.lives --;
                    uiManager.updateLives();

                    // Remove any active effects so that the player doesn't respawn with the effects active
                    Player.RemoveActiveEffects();
                    if (Player.lives <= 0) {
                        // Game over animation
                        gameObject.SetActive(false);
                        uiManager.playDeathAnimation();
                    } else {
                        // Respawn the player at either a checkpoint if they've obtained one or at the original spawn point
                        gameObject.transform.position = Player.checkpointObtained ? Player.lastCheckPoint : Player.spawnPoint;
                    }
                    break;
                case "Checkpoint":
                    Player.score += 500;
                    uiManager.updateScore();

                    MeshRenderer checkpointRenderer = objCollider.GetComponent<MeshRenderer>();
                    if (checkpointRenderer.enabled) {
                        checkpointRenderer.enabled = false;
                        objCollider.enabled = false;
                        Player.lastCheckPoint = objCollider.transform.position;
                        Player.checkpointObtained = true;
                    }
                    break;
                case "Ring":
                    bool isInTheMiddle = Mathf.Round((objCollider.transform.position - transform.position).magnitude) == 0;

                    if (isInTheMiddle) {
                        Player.score += 500;
                        uiManager.updateScore();
                        uiManager.obtainRing(objCollider);
                    }
                    break;
                case "Fly":
                    uiManager.resetProgressBar();
                    Player.ActiveEffects.Add(Fly);
                    Player.Effects[Fly].startTime();
                    break;
                case "Fly Press":
                    uiManager.resetProgressBar();
                    Player.ActiveEffects.Add(FlyPress);
                    Player.Effects[FlyPress].startTime();
                    break;
                case "Speed":
                    uiManager.resetProgressBar();
                    Player.ActiveEffects.Add(Speed);
                    Player.Effects[Speed].startTime();
                    break;
                case "Health":
                    Player.score += 1000;
                    Player.lives ++;
                    uiManager.updateLives();
                    uiManager.updateScore();

                    // Remove the life bubble
                    objCollider.gameObject.SetActive(false);
                    break;
                case "Open End":
                    // Increase the score according to the number of lives left
                    gameObject.SetActive(false);
                    Player.score += 1000 * Player.lives;
                    uiManager.updateScore();
                    uiManager.playWinningAnimation();
                    break;
                case "Water":
                    playerBody.drag = 2;
                    if (Mathf.RoundToInt(gameObject.transform.localScale.magnitude) == Player.Big) {
                        float topBrimPosition = objCollider.bounds.extents.y + objCollider.transform.position.y - 1.5f;
                        float distanceFromTopBrim = transform.position.y - topBrimPosition;
                        Vector3 buoyancyForce = new Vector3(
                            0,
                            2 * PlayerMovementManager.GRAVITY,
                            0
                        );
                        if (distanceFromTopBrim >= 0) buoyancyForce.y -= (8f/21f) * distanceFromTopBrim;
                        playerBody.AddForce(buoyancyForce);
                    }
                    break;
            }
        }

        private void FixedUpdate() {
            float largestProgressValue = 1f;

            // Iterate through each effect and determine each effect should still be applied to the player
            // depending on its duration
            foreach (Effects effects in Player.ActiveEffects) {
                Effect effect = Player.Effects[effects];
                // Get the current progress value for the current effect
                var currentProgressValue = effect.progress();
                if (currentProgressValue < 1f) {
                    if (currentProgressValue < largestProgressValue) largestProgressValue = currentProgressValue;
                    // Enact the effect so that it affects the player
                    effect.enact();
                } else {
                    // Remove the effects of the effect from the player
                    effect.desist();
                    // Flag the effect in the list of active effects to be removed
                    expiredEffects.Add(effects);
                }
            }
            // Remove the expired effects from the set of active effects
            Player.ActiveEffects.ExceptWith(expiredEffects);
            expiredEffects.Clear();

            // Update the progress bar to display the remaining time for the newest effect
            uiManager.setProgressBar(largestProgressValue);
        }
    }
}