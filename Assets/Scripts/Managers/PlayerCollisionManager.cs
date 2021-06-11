using System.Collections.Generic;
using UnityEngine;
using static Effects;

public class PlayerCollisionManager : MonoBehaviour {
    public UIManager UIManager;
    public Rigidbody playerBody;

    private HashSet<Effects> expiredEffects = new HashSet<Effects>();


    private void Start() {
        // Initialise all of the player values
        Player.Init();
    }

    private void OnCollisionEnter(Collision collision) {
        handleCollider(collision.collider);
    }

    private void OnTriggerEnter(Collider collider) {
        handleCollider(collider);
    }
    
    private void OnTriggerStay(Collider collider) {
        handleCollider(collider);
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.tag == "Water") playerBody.drag = 0;
    }

    private void handleCollider(Collider collider) {
        switch (collider.tag) {
            case "Deflator":
                // Only deflate the ball if it is big
                if (Mathf.Round(gameObject.transform.localScale.magnitude) == Player.Big)
                    gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case "Inflator":
                // Only inflate the ball if it is small
                if (Mathf.Round(gameObject.transform.localScale.magnitude) == Player.Small)
                    gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                break;
            case "Pop":
                Player.lives --;
                UIManager.updateLives();

                // Remove any active effects so that the player doesn't respawn with the effects active
                Player.RemoveActiveEffects();
                if (Player.lives <= 0) {
                    // Game over animation
                    gameObject.SetActive(false);
                    UIManager.playDeathAnimation();
                } else {
                    // Respawn the player at either a checkpoint if they've obtained one or at the original spawn point
                    gameObject.transform.position = Player.checkpointObtained ? Player.lastCheckPoint : Player.spawnPoint;
                }
                break;
            case "Checkpoint":
                Player.score += 500;
                UIManager.updateScore();

                MeshRenderer checkpointRenderer = collider.GetComponent<MeshRenderer>();
                if (checkpointRenderer.enabled) {
                    checkpointRenderer.enabled = false;
                    collider.enabled = false;
                    Player.lastCheckPoint = collider.transform.position;
                    Player.checkpointObtained = true;
                }
                break;
            case "Ring":
                bool isInTheMiddle = Mathf.Round((collider.transform.position - transform.position).magnitude) == 0;

                if (isInTheMiddle) {
                    Player.score += 500;
                    UIManager.updateScore();
                    UIManager.obtainRing(collider);
                }
                break;
            case "Fly":
                UIManager.resetProgressBar();
                Player.ActiveEffects.Add(Fly);
                Player.Effects[Fly].startTime();
                break;
            case "Fly Press":
                UIManager.resetProgressBar();
                Player.ActiveEffects.Add(FlyPress);
                Player.Effects[FlyPress].startTime();
                break;
            case "Speed":
                UIManager.resetProgressBar();
                Player.ActiveEffects.Add(Speed);
                Player.Effects[Speed].startTime();
                break;
            case "Health":
                Player.score += 1000;
                Player.lives ++;
                UIManager.updateLives();
                UIManager.updateScore();

                // Remove the life bubble
                collider.gameObject.SetActive(false);
                break;
            case "Open End":
                // Increase the score according to the number of lives left
                gameObject.SetActive(false);
                Player.score += 1000 * Player.lives;
                UIManager.updateScore();
                UIManager.playWinningAnimation();
                break;
            case "Water":
                playerBody.drag = 2;
                if (Mathf.Round(gameObject.transform.localScale.magnitude) == Player.Big) {
                    float topBrimPosition = collider.bounds.extents.y + collider.transform.position.y - 1.5f;
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
            default: break;
        }
    }

    private void FixedUpdate() {
        float largestProgressValue = 1f, currentProgressValue;

        // Iterate through each effect and determine each effect should still be applied to the player
        // depening on its duration
        foreach (Effects effects in Player.ActiveEffects) {
            Effect effect = Player.Effects[effects];
            // Get the current progress value for the current effect
            currentProgressValue = effect.progress();
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
        UIManager.setProgressBar(largestProgressValue);
    }
}