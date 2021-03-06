using System.Collections.Generic;
using Structural;
using UnityEngine;
using static Structural.Effects;

namespace Managers  {
    public class PlayerCollisionManager : MonoBehaviour {
        public UIManager uiManager;
        public Rigidbody playerBody;

        private readonly HashSet<Effects> expiredEffects = new HashSet<Effects>();

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
        
        private void FixedUpdate() {
            float largestProgressValue = 1f;

            // Iterate through each effect and determine each effect should still be applied to the player
            // depending on its duration
            foreach (Effects effects in Player.ActiveEffects) {
                Effect effect = Player.Effects[effects];
                // Get the current progress value for the current effect
                var currentProgressValue = effect.Progress();
                if (currentProgressValue < 1f) {
                    if (currentProgressValue < largestProgressValue) largestProgressValue = currentProgressValue;
                    // Enact the effect so that it affects the player
                    effect.Enact();
                } else {
                    // Remove the effects of the effect from the player
                    effect.Desist();
                    // Flag the effect in the list of active effects to be removed
                    expiredEffects.Add(effects);
                }
            }
            // Remove the expired effects from the set of active effects
            Player.ActiveEffects.ExceptWith(expiredEffects);
            expiredEffects.Clear();

            // Update the progress bar to display the remaining time for the newest effect
            uiManager.SetProgressBar(largestProgressValue);
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
                    uiManager.UpdateLives();

                    // Remove any active effects so that the player doesn't respawn with
                    // the effects active and also deactivate the player
                    Player.RemoveActiveEffects();
                    gameObject.SetActive(false);
                    
                    // Depending on the number of lives left; respawn or game over
                    if (Player.limitedLives && Player.lives <= 0)
                        uiManager.PlayGameOverAnimation();
                    else
                        Invoke(nameof(Respawn), 1.25f);
                    
                    break;
                case "Checkpoint":
                    Player.score += 500;
                    uiManager.UpdateScore();

                    MeshRenderer checkpointRenderer = objCollider.GetComponent<MeshRenderer>();
                    if (checkpointRenderer.enabled) {
                        checkpointRenderer.enabled = false;
                        objCollider.enabled = false;
                        Player.spawnPoint = objCollider.transform.position;
                    }
                    break;
                case "Ring":
                    bool isInTheMiddle = Mathf.Round((objCollider.transform.position - transform.position).magnitude) == 0;

                    if (isInTheMiddle) {
                        Player.score += 500;
                        uiManager.UpdateScore();
                        uiManager.ObtainRing(objCollider);
                    }
                    break;
                case "Fly":
                    uiManager.ResetProgressBar();
                    Player.ActiveEffects.Add(Fly);
                    Player.Effects[Fly].StartTime();
                    break;
                case "Fly Press":
                    uiManager.ResetProgressBar();
                    Player.ActiveEffects.Add(FlyPress);
                    Player.Effects[FlyPress].StartTime();
                    break;
                case "Speed":
                    uiManager.ResetProgressBar();
                    Player.ActiveEffects.Add(Speed);
                    Player.Effects[Speed].StartTime();
                    break;
                case "Health":
                    Player.score += 1000;
                    Player.lives ++;
                    uiManager.UpdateLives();
                    uiManager.UpdateScore();

                    // Remove the life bubble
                    objCollider.gameObject.SetActive(false);
                    break;
                case "Open End":
                    gameObject.SetActive(false);
                    
                    // Only increase the score according to the number of lives
                    // left, if the player doesn't have unlimited lives
                    if (Player.limitedLives) Player.score += 1000 * Player.lives;
                    
                    uiManager.UpdateScore();
                    uiManager.PlayWinningAnimation();
                    break;
                case "Water":
                    playerBody.drag = 2;
                    if (Mathf.RoundToInt(gameObject.transform.localScale.magnitude) == Player.Big) {
                        float topBrimPosition = objCollider.bounds.extents.y + objCollider.transform.position.y - 1.5f;
                        float distanceFromTopBrim = transform.position.y - topBrimPosition;
                        Vector3 buoyancyForce = new Vector3(
                            0,
                            2 * PlayerMovementManager.Gravity,
                            0
                        );
                        if (distanceFromTopBrim >= 0) buoyancyForce.y -= 10 * distanceFromTopBrim;
                        playerBody.AddForce(buoyancyForce);
                    }
                    break;
                case "Platform":
                    transform.SetParent(objCollider.transform);
                    break;
            }
        }

        private void Respawn() {
            // Respawn the player at either a checkpoint if they've obtained one or at the original spawn point
            gameObject.transform.position = Player.spawnPoint;
            gameObject.SetActive(true);
        }
    }
}